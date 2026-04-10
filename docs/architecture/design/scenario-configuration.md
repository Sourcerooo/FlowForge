# Scenario and Configuration Design

This document is normative for process configuration, scenario loading direction, and runtime tracking identifiers.

## Canonical Process Model

The canonical internal process model should live in `FlowForge.Domain`, not in `FlowForge.Simulation`.

Recommended baseline shape:

```csharp
public sealed record ProcessConfiguration(
    Guid ProcessConfigurationId,
    string ProcessKey,
    string Name,
    TimeSpan PlannedDuration,
    ArrivalProfileDefinition ArrivalProfile,
    IReadOnlyList<StageDefinition> Stages);

public sealed record ArrivalProfileDefinition(
    TimeSpan GenerationWindow,
    int AverageWorkItemsPerWindow,
    int? MaxWorkItemsPerWindow);

public sealed record StageDefinition(
    Guid StageId,
    string StageKey,
    string DisplayName,
    int Sequence,
    IReadOnlyList<StationDefinition> Stations);

public sealed record StationDefinition(
    Guid StationId,
    Guid StageId,
    string StationKey,
    string DisplayName,
    int WorkerCount,
    TimeSpan AverageProcessingTime);
```

## Invariants

- `ProcessConfiguration` is immutable after import
- stages are ordered by `Sequence`
- `Sequence` values are unique within one process
- each stage contains at least one station
- each station belongs to exactly one stage
- internal runtime identities are GUID-based and generated during import
- external authored keys remain for traceability and diagnostics

## Scenario Authoring Direction

The current runtime slice stays in-memory first.
When external scenario persistence is introduced, the preferred authored format is hierarchical JSON.

Recommended example:

```json
{
  "scenarioKey": "default-fulfillment",
  "name": "Default Fulfillment Flow",
  "plannedDuration": "1.00:00:00",
  "arrivalProfile": {
    "generationWindow": "00:15:00",
    "averageWorkItemsPerWindow": 25,
    "maxWorkItemsPerWindow": 40
  },
  "stages": {
    "picking": {
      "displayName": "Picking",
      "sequence": 10,
      "stations": {
        "pick-a": {
          "displayName": "Pick A",
          "workerCount": 2,
          "averageProcessingTime": "00:03:00"
        }
      }
    }
  }
}
```

Recommended interpretation:

- authored files provide stable external keys, not internal IDs
- validation rejects duplicate stage and station keys before GUID generation
- imported GUIDs are the only IDs used inside runtime state, tracking, events, and snapshots

## Loading and Ownership Flow

```text
Scenario JSON file
  -> Infrastructure JSON loader
  -> imported persistence model
  -> Infrastructure mapping into Domain ProcessConfiguration
  -> Application orchestration
  -> Simulation runtime consumption
```

Recommended ownership split:

- `Infrastructure` loads and validates raw JSON
- `Infrastructure` maps into domain-owned `ProcessConfiguration`
- `Application` orchestrates loading and run start
- `Simulation` consumes the domain-owned configuration directly

## Work-Item Tracking Model

The recommended tracking direction is segment-based tracking instead of one timestamp set per stage.

Recommended shape:

```csharp
public sealed class WorkItemTracking
{
    public Guid TrackingSubjectId { get; init; }
    public long CurrentProcessingToken { get; private set; }
    public TimeSpan CreatedAt { get; init; }
    public TimeSpan? CompletedAt { get; private set; }
    public WorkItemStatus CurrentStatus { get; private set; }
    public Guid? CurrentStageId { get; private set; }
    public IReadOnlyList<WorkItemTrackingSegment> Segments { get; }
    public TimeSpan? TotalLeadTime { get; private set; }
}

public sealed class WorkItemTrackingSegment
{
    public long SegmentId { get; init; }
    public long ProcessingToken { get; init; }
    public Guid? StageId { get; init; }
    public Guid StationId { get; init; }
    public TrackingSegmentType SegmentType { get; init; }
    public TimeSpan StartedAt { get; init; }
    public TimeSpan? EndedAt { get; private set; }
    public TimeSpan? Duration { get; private set; }
    public string? Reason { get; init; }
}
```

Recommended meaning:

- each queue wait, processing run, on-hold period, or later transfer is a separate segment
- repeated visits to a stage remain representable without redesigning the model
- snapshots do not need to expose the full segment list by default

Recommended consequences:

- queue waiting is stage-scoped because one stage owns one shared backlog for all of its stations
- a work item should not change queue segments merely because the eventual executing station changes
- putting a work item on hold closes the active queue or processing segment and opens an `OnHold` segment
- resuming from hold closes the `OnHold` segment and opens the next queue or processing segment
- future rework remains representable without changing the model shape

Recommended tracking subject direction:

```csharp
public sealed class TrackingSubjectReference
{
    public Guid TrackingSubjectId { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public Guid ExternalEntityId { get; init; }
    public string? SourceSystem { get; init; }
}

public enum TrackingSegmentType
{
    QueueWait = 0,
    Processing = 1,
    OnHold = 2,
    Transfer = 3
}
```

Meaning:

- `TrackingSubjectId` is the simulation-facing identity
- a separate registry maps it to the concrete domain or master-data object when needed
- total durations for a stage are derived by summing matching segments instead of assuming exactly one visit

## Station and Stage Tracking

Recommended direction:

- `StationTracking` owns cumulative facts for one concrete station
- `StageTracking` owns shared stage queue facts and aggregates over all stations belonging to the same stage
- KPI calculations can answer both stage-level and concrete-station questions

Recommended shapes:

```csharp
public sealed class StationTracking
{
    public Guid StationId { get; init; }
    public Guid StageId { get; init; }
    public long WorkItemsStartedCount { get; private set; }
    public long WorkItemsCompletedCount { get; private set; }
    public long WorkItemsPlacedOnHoldCount { get; private set; }
    public long WorkItemsRequeuedCount { get; private set; }
    public TimeSpan CumulativeProcessingTime { get; private set; }
    public TimeSpan CumulativeOnHoldTime { get; private set; }
    public TimeSpan CumulativeBusyTime { get; private set; }
    public int PeakBusyWorkers { get; private set; }
}

public sealed class StageTracking
{
    public Guid StageId { get; init; }
    public long WorkItemsQueuedCount { get; private set; }
    public TimeSpan CumulativeQueueWait { get; private set; }
    public int CurrentQueueLength { get; private set; }
    public int PeakQueueLength { get; private set; }
    public IReadOnlyDictionary<Guid, StationTracking> Stations { get; }
}
```

Recommended meaning:

- `StationTracking` owns cumulative facts for one concrete processing resource
- `StageTracking` owns the shared queue behavior of the stage and aggregates over multiple stations of the same logical stage
- this enables both questions: how overloaded is the full stage queue, and how utilized is one concrete station

## Stage Runtime Queue

Recommended direction:

- the shared queue should be owned by a runtime object such as `StageRuntimeState`, not by `StageTracking`
- `StageTracking` keeps cumulative facts and current queue counters for metrics, but not the authoritative queued item collection
- `StationTracking` should not own queue membership because stations are business-equivalent execution areas inside a stage

Recommended baseline shape:

```csharp
public sealed class StageRuntimeState
{
    public Guid StageId { get; init; }
    public IReadOnlyDictionary<Guid, StationRuntimeState> Stations { get; }
    public IReadOnlyCollection<StageQueueEntry> Queue { get; }
}

public sealed record StageQueueEntry(
    Guid TrackingSubjectId,
    TimeSpan QueuedAt,
    long QueueSequenceNumber,
    long WorkItemVersion);
```

Recommended queue content:

- store `StageQueueEntry` values or an equivalent lightweight work-item reference object
- include the work-item identity and queue-entered time so FIFO and queue-wait calculations remain deterministic
- include a queue sequence or version field so stale queue entries can be detected after requeue or invalidation scenarios
- do not store scheduled events inside the stage queue because events belong to the simulation scheduler, not to the operational backlog

## Aggregation Rules

- work-item tracking stores exact factual segment history
- station tracking aggregates facts per concrete station
- stage tracking aggregates shared queue facts and station facts for the same logical stage
- KPI collector consumes aggregates rather than replaying full work-item history on every publish

Recommended examples:

- total queue time for one work item in one stage is the sum of all `QueueWait` segments matching the same `StageId`
- queue wait is stage-level by default and should not depend on which concrete station later processes the work item
- total on-hold time is the sum of all `OnHold` segments across all stages
- average queue wait for a stage is the sum of all stage-level `QueueWait` durations divided by started-processing count in that stage

## Update Ownership by Event

Recommended update ownership:

| Event | `SimulationState` | `WorkItemTracking` | `StationTracking` | `IKpiCollector` |
|---|---|---|---|---|
| `GenerateSimulationEvent` | creates incoming work items | creates new tracking entries | no direct update | increments created and WIP counters if work items are materialized here |
| `WorkItemQueueEvent` | pushes a work item into the target stage queue and updates current status or stage | closes prior transfer or hold segment if needed and opens a new `QueueWait` segment | no direct update yet beyond later station assignment | may refresh current bottleneck inputs |
| `ProcessingStartEvent` | reserves worker capacity and updates active processing state | closes the active `QueueWait` segment, opens a `Processing` segment, increments `CurrentProcessingToken` | increments started count, updates busy-worker peak | updates live stage activity metrics including realized queue wait |
| `ProcessingCompleteEvent` | releases worker capacity and updates runtime state | closes the active `Processing` segment and records realized processing duration | increments completed count, adds processing duration and busy time | updates stage metrics and lead-time inputs when relevant |
| `WorkItemCompleteEvent` | marks the runtime work item complete or removes active runtime presence | sets completion markers and total lead time | no direct update beyond previous completion metrics | increments completed count, updates lead-time aggregates, updates WIP |

Recommended rule:

- factual timestamps are written during queue, start, and completion handling
- requeue, hold, and resume behavior is modeled as closing one segment and opening the next one
- snapshot publication does not reconstruct missing queue or processing durations retroactively
