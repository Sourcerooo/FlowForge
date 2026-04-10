# Simulation Orchestration Design

This document is normative for runtime-object boundaries, process orchestration, and multi-object flow sequencing.

## Work-Item States

Recommended MVP state machine:

- `Created`
- `QueuedForPicking`
- `Picking`
- `QueuedForPacking`
- `Packing`
- `QueuedForShipping`
- `Shipping`
- `Completed`

Recommended rule:

- queue states are explicit
- processing states are explicit
- movement between stages is represented by queue, start, and completion events; station assignment happens only when a stage dispatches work to free capacity

## Orchestration Boundary

Event handlers stay thin and delegate process-step coordination to `IWorkItemProcessOrchestrator`.

Recommended split:

| Component | Owns |
|---|---|
| `ISimulationEventHandler` | event entry point |
| `IWorkItemProcessOrchestrator` | cross-object coordination for one process step |
| `StageRuntimeState` | shared stage queue, station availability view, and dispatch decisions |
| `WorkItemRuntimeState` | current runtime head state |
| `WorkItemTracking` | factual timing and segment history |
| `StationTracking` | station-level processing counters, capacity, and durations |
| `StageTracking` | stage-level queue facts and aggregates across stations |
| `IWorkItemTransitionPolicy` | transition legality |
| `IProcessRoutingPolicy` | next-step routing |
| `IKpiCollector` | incremental KPI facts |

Recommended heuristic:

- local, consistency-protecting state changes belong on the object that owns the data
- sequencing across multiple runtime objects belongs in the orchestrator

## Process Orchestrator Contracts

Recommended baseline shape:

```csharp
public interface IWorkItemProcessOrchestrator
{
    Task CreateFromGenerationAsync(
        CreateWorkItemFromGenerationCommand command,
        CancellationToken cancellationToken = default);

    Task QueueForStageAsync(
        QueueWorkItemCommand command,
        CancellationToken cancellationToken = default);

    Task StartProcessingAsync(
        StartProcessingCommand command,
        CancellationToken cancellationToken = default);

    Task CompleteProcessingAsync(
        CompleteProcessingCommand command,
        CancellationToken cancellationToken = default);

    Task CompleteWorkItemAsync(
        CompleteWorkItemCommand command,
        CancellationToken cancellationToken = default);
}

public interface IWorkItemTransitionPolicy
{
    void EnsureCanQueue(WorkItemRuntimeState workItem, QueueWorkItemCommand command);
    void EnsureCanStartProcessing(WorkItemRuntimeState workItem, StartProcessingCommand command);
    void EnsureCanCompleteProcessing(WorkItemRuntimeState workItem, CompleteProcessingCommand command);
    void EnsureCanCompleteWorkItem(WorkItemRuntimeState workItem, CompleteWorkItemCommand command);
}

public interface IProcessRoutingPolicy
{
    RoutingDecision GetNextStep(
        ProcessConfiguration processConfiguration,
        StageId completedStageId);
}
```

## Command Direction

```csharp
public sealed record QueueWorkItemCommand(
    SimulationRunId SimulationRunId,
    TrackingSubjectId TrackingSubjectId,
    TimeSpan OccurredAt,
    StageId StageId);

public sealed record StartProcessingCommand(
    SimulationRunId SimulationRunId,
    TrackingSubjectId TrackingSubjectId,
    TimeSpan OccurredAt,
    StageId StageId,
    StationId StationId,
    long ProcessingToken);

public sealed record CompleteProcessingCommand(
    SimulationRunId SimulationRunId,
    TrackingSubjectId TrackingSubjectId,
    TimeSpan OccurredAt,
    StageId StageId,
    StationId StationId,
    long ProcessingToken);
```

## Runtime-Object Method Direction

```csharp
public sealed class WorkItemRuntimeState
{
    public void QueueForStage(TimeSpan occurredAt, StageId stageId);
    public void StartProcessing(TimeSpan occurredAt, StageId stageId, StationId stationId, long processingToken);
    public void CompleteProcessing(TimeSpan occurredAt, StageId stageId, StationId stationId, long processingToken);
    public void CompleteWorkItem(TimeSpan occurredAt);
}

public sealed class StageRuntimeState
{
    public StageId StageId { get; }
    public IReadOnlyDictionary<StationId, StationRuntimeState> Stations { get; }
    public IReadOnlyCollection<StageQueueEntry> Queue { get; }

    public void Enqueue(StageQueueEntry entry);
    public StationId? TryReserveFreeStation(TimeSpan occurredAt);
    public StageQueueEntry DequeueNext();
}

public sealed record StageQueueEntry(
    TrackingSubjectId TrackingSubjectId,
    TimeSpan QueuedAt,
    long QueueSequenceNumber,
    long WorkItemVersion);
```

Recommended rule:

- `StageRuntimeState` owns the shared backlog for all business-equivalent stations of one stage
- station assignment is a runtime dispatch decision, not a queue placement decision
- avoid technical setter-style APIs for runtime mutation
- use explicit process methods that complete one valid transition in one step
- this reduces inconsistent intermediate state across work item, tracking, station, and stage objects

## Orchestration Flow

Recommended handler-to-orchestrator flow:

```text
SimulationRunner
  -> IEventDispatcher
  -> ProcessingCompleteEventHandler
  -> IWorkItemProcessOrchestrator.CompleteProcessingAsync(command)
  -> StageRuntimeState / WorkItemRuntimeState / WorkItemTracking / StationTracking / StageTracking
  -> IKpiCollector
  -> ISimulationScheduler
```

Recommended `CompleteProcessing` sequence:

1. load the current `StageRuntimeState`, `WorkItemRuntimeState`, `WorkItemTracking`, `StationTracking`, and `StageTracking`
2. validate stage, station, current status, and `ProcessingToken` through `IWorkItemTransitionPolicy`
3. call `WorkItemRuntimeState.CompleteProcessing(...)`
4. call `WorkItemTracking.CompleteProcessing(...)`
5. call `StationTracking.CompleteProcessing(...)`
6. call `StageTracking.CompleteProcessing(...)`
7. call `IKpiCollector` to record incremental KPI facts
8. resolve the next route through `IProcessRoutingPolicy`
9. schedule the next `WorkItemQueueEvent` or terminal `WorkItemCompleteEvent` through `ISimulationScheduler`

Recommended `QueueForStage` sequence:

1. load the current `StageRuntimeState`, `WorkItemRuntimeState`, `WorkItemTracking`, and `StageTracking`
2. validate that the work item may enter the requested stage
3. append a `StageQueueEntry` to the stage-owned queue
4. call `WorkItemRuntimeState.QueueForStage(...)`
5. open a stage-level `QueueWait` segment on `WorkItemTracking`
6. update stage-level queue facts on `StageTracking`
7. if `StageRuntimeState` can reserve a free station worker, dequeue the oldest eligible `StageQueueEntry`
8. schedule `ProcessingStartEvent` with the selected `StationId`

Recommended modeling rule:

- the handler remains a thin event adapter
- the orchestrator owns use-case sequencing
- runtime objects own local mutation and invariants
- stage queues are runtime state, not tracking history and not event-scheduler entries
- follow-up events are coordinated through the scheduler and never by direct queue mutation
