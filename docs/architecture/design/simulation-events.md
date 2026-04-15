# Simulation Events Design

This document is normative for the event family, base event contracts, ordering semantics, and invalidation rules.

## Event Family

The current recommended MVP direction is a generic event family with routing metadata.

| Event | Purpose | Typical producer | Typical follow-up |
|---|---|---|---|
| `GenerateSimulationEvent` | Generate incoming work items for a time slice | bootstrap or generator | create work items, enqueue queue events, schedule next generation |
| `WorkItemQueueEvent` | Place a work item into a stage queue | generator or routing logic | try to dispatch to free stage capacity |
| `ProcessingStartEvent` | Start processing at a station | stage dispatch logic | schedule matching completion |
| `ProcessingCompleteEvent` | Finish processing at a station | runner after scheduled delay | route to next station or complete work item |
| `WorkItemCompleteEvent` | Finalize the work item lifecycle | final-stage completion logic | update KPIs and counters |
| `SnapshotPublishedEvent` | Trigger snapshot publication | snapshot policy | build and publish immutable snapshot |

Advantages:

- extensible to additional stages without multiplying CLR event types
- consistent queue, start, and completion semantics across stations
- future-compatible with disruptions, shifts, gates, and maintenance events

Trade-off:

- routing uses contextual metadata rather than only CLR type

## Event Handling Semantics

- `GenerateSimulationEvent` should be the first event scheduled when a simulation starts
- the generation handler creates work items for the configured time slice
- new work items enter the run in state `Created` and then enqueue the first `WorkItemQueueEvent`
- `WorkItemQueueEvent` carries target stage context, appends the work item to the shared stage queue, and triggers capacity checks across all stations of that stage
- `ProcessingStartEvent` reserves capacity, stamps start time, increments the processing token, and schedules the completion event
- `ProcessingCompleteEvent` validates its processing token before mutating state
- a valid `ProcessingCompleteEvent` releases capacity, updates counters and history, and enqueues the next queue or completion event
- `WorkItemCompleteEvent` finalizes timestamps, counters, and KPI contributions

## Queue Ordering Contract

The priority queue should order events by the following fields in this order:

1. `ScheduledTime`
2. `SortRank`
3. `SequenceNumber`

Meaning:

- earlier simulation time always wins
- for the same time, higher-priority event classes run first
- for the same time and priority, lower sequence number wins for determinism

Recommended default priority bands:

| Sort rank band | Purpose |
|---|---|
| `10` | completion-style events |
| `20` | queueing and routing events |
| `30` | start events |
| `40` | generation and maintenance events |
| `50` | snapshot publication |

Recommended default ordering at the same `ScheduledTime`:

1. `ProcessingCompleteEvent`
2. `WorkItemCompleteEvent`
3. future interruption events such as disruptions
4. `WorkItemQueueEvent`
5. `ProcessingStartEvent`
6. `GenerateSimulationEvent`
7. `SnapshotPublishedEvent`

Reasoning:

- completion happens before new starts so released capacity is visible immediately
- queueing happens before start attempts so newly routed work is visible to stage dispatch logic
- generation runs after current-cycle completions and routing
- snapshot publication usually observes the already-applied state for that simulated timestamp

## Base Event Direction

Recommended baseline shape:

```csharp
public abstract record SimulationEvent(
    Guid EventId,
    Guid SimulationRunId,
    long SequenceNumber,
    EventSortRank SortRank,
    EventKind EventKind,
    ProcessStage ProcessStage,
    TimeSpan ScheduledTime,
    Guid? OrderId,
    long? ProcessingToken,
    string? SubKind = null);
```

Interpretation of the most important fields:

- `EventId` is a technical identifier for tracing and debugging
- `SimulationRunId` scopes all events to one full simulation execution
- `SequenceNumber` is assigned centrally by the scheduler and guarantees deterministic ordering
- `SortRank` defines the cross-cutting execution order for event classes
- `EventKind` identifies the generic event family
- `ProcessStage` identifies the targeted process stage
- `OrderId` is optional because generator or maintenance events may not target one concrete work item
- `ProcessingToken` is required for execution events so outdated completions can be skipped
- `SubKind` is an extension hook for future specialization

## Enum Direction

Recommended enums for the first implementation:

```csharp
public enum EventKind
{
    Generate = 0,
    OrderQueued = 1,
    ProcessingStarted = 2,
    ProcessingCompleted = 3,
    OrderCompleted = 4,
    SnapshotPublished = 5,
    DisruptionRaised = 100,
    DisruptionCleared = 101
}

public enum ProcessStage
{
    None = 0,
    Picking = 1,
    Packing = 2,
    Shipping = 3
}

public enum EventSortRank
{
    Highest = 0,
    Completion = 10,
    Routing = 20,
    Start = 30,
    Generation = 40,
    Snapshot = 50,
    Lowest = 100
}
```

Recommended rules:

- `EventKind` models semantic intent, not execution order
- `ProcessStage` stays small and business-readable until fully replaced by configured stage identities
- `EventSortRank` makes ordering policy explicit instead of hiding it in arbitrary numbers

## Minimal Payload Direction

All simulation events should likely share a common base payload such as:

| Field | Purpose |
|---|---|
| `EventId` | Unique technical identifier for tracing and deterministic ordering |
| `ScheduledTime` | Simulation timestamp at which the event becomes due |
| `SortRank` | Tie-breaker when several events share the same simulation time |
| `SequenceNumber` | Preserves deterministic ordering when time and priority are equal |
| `SimulationRunId` | Correlates events to one simulation run |
| `ProcessingToken` | Correlates an event to the current processing run or version |
| `EventKind` | Identifies the generic event family used for routing |
| `ProcessStage` | Identifies the targeted stage such as Picking, Packing, or Shipping |

Stage-, station-, and work-item-related events should additionally carry targeted business data such as:

| Event family | Suggested payload |
|---|---|
| `GenerateSimulationEvent` | generation window start or end, batch settings reference, optional scenario snapshot or version |
| `OrderQueuedEvent` | `OrderId`, target `StageId`, queue-entered timestamp |
| `ProcessingStartedEvent` | `OrderId`, `StationId`, assigned worker slot or capacity token, processing duration |
| `ProcessingCompletedEvent` | `OrderId`, `StationId`, processing-start timestamp, processing-end timestamp |
| `OrderCompletedEvent` | `OrderId`, completion timestamp |

Recommended default:

- keep event payloads small and explicit
- do not place full aggregate snapshots inside events
- prefer identifiers plus the minimal deterministic data needed by the handler
- let `SimulationState` remain the source of broader runtime context

Recommended queue payload rule:

- stage queues should store work-item references and queue metadata, not scheduled events
- the event queue models future execution intent, while the stage queue models current operational backlog
- `WorkItemQueueEvent` should therefore enqueue a `StageQueueEntry` or equivalent runtime payload, then let stage dispatch logic create `ProcessingStartEvent`

## Invalidation and Skip Model

The recommended approach is version-based invalidation through `ProcessingToken`.

Rules:

- each work item keeps a mutable current `ProcessingToken` in runtime state
- `ProcessingStartEvent` increments the token when work actually begins
- the matching `ProcessingCompleteEvent` stores that token
- if a completion event is dequeued with a stale token, it is skipped without mutating state

This supports later cases such as:

- process interruption after a started operation
- work being re-queued with a new run or version
- cancellation or replacement of a previously scheduled completion

Skip behavior:

- log at debug or trace level if useful
- count stale events in diagnostics if useful
- never mutate state, tracking, or KPIs for stale completions
- do not treat stale completion events as runtime failures
