# Simulation Execution Context Design

This document is normative for run-scoped execution context, handler-facing context, and the boundary between runtime data and injected collaborators.

## Execution Context Ownership

One simulation run owns one root execution context.
Handlers receive a narrower handler-facing context so the raw queue remains hidden.

Recommended owner and creation boundary:

- application orchestrates start
- simulation-side factory builds the full runtime data context once per run
- handlers do not create or replace the context

## Root Context Shape

Recommended shape:

```csharp
public sealed class SimulationExecutionContext
{
    public Guid SimulationRunId { get; init; }
    public ProcessConfiguration ProcessConfiguration { get; init; } = default!;
    public SimulationMetadata Metadata { get; init; } = default!;
    public SimulationState State { get; init; } = default!;
    public ISimulationEventQueue EventQueue { get; init; } = default!;
    public ITrackingSubjectStore TrackingSubjectStore { get; init; } = default!;
    public IWorkItemTrackingStore WorkItemTrackingStore { get; init; } = default!;
    public IStationTrackingStore StationTrackingStore { get; init; } = default!;
    public ISnapshotStore SnapshotStore { get; init; } = default!;
    public ISnapshotTimelineStore SnapshotTimelineStore { get; init; } = default!;

    public SimulationExecutionHandlerContext CreateHandlerContext() => new()
    {
        SimulationRunId = SimulationRunId,
        ProcessConfiguration = ProcessConfiguration,
        Metadata = Metadata,
        State = State,
        TrackingSubjectStore = TrackingSubjectStore,
        WorkItemTrackingStore = WorkItemTrackingStore,
        StationTrackingStore = StationTrackingStore,
        SnapshotStore = SnapshotStore,
        SnapshotTimelineStore = SnapshotTimelineStore
    };
}
```

## Handler-Facing Context Shape

```csharp
public sealed class SimulationExecutionHandlerContext
{
    public Guid SimulationRunId { get; init; }
    public ProcessConfiguration ProcessConfiguration { get; init; } = default!;
    public SimulationMetadata Metadata { get; init; } = default!;
    public SimulationState State { get; init; } = default!;
    public ITrackingSubjectStore TrackingSubjectStore { get; init; } = default!;
    public IWorkItemTrackingStore WorkItemTrackingStore { get; init; } = default!;
    public IStationTrackingStore StationTrackingStore { get; init; } = default!;
    public ISnapshotStore SnapshotStore { get; init; } = default!;
    public ISnapshotTimelineStore SnapshotTimelineStore { get; init; } = default!;
}
```

Visibility rule:

- `SimulationExecutionContext` is for runner and factory internals
- `SimulationExecutionHandlerContext` is the only context shape handed to event handlers
- this keeps dequeue access out of handler APIs even if one concrete queue object implements both queue and scheduler interfaces

## Context Interpretation

- `SimulationRunId` identifies one concrete execution and scopes queue entries, snapshots, and diagnostics
- `ProcessConfiguration` is the immutable process topology and arrival definition for the run
- `Metadata` holds run-scoped descriptive information that is not part of mutable execution state
- `State` holds mutable simulation state such as current simulated time, active work items, and station occupancy
- `EventQueue` is the run-local mutable priority queue and is runner-facing only
- tracking and snapshot stores are run-scoped data collaborators owned by the live execution

Explicit non-goal:

- `SimulationExecutionContext` must not become a generic service bag
- scheduler, dispatcher, event handlers, orchestrators, KPI collector, and snapshot builder should be injected into the classes that own behavior whenever practical

## Factory Direction

```csharp
public interface ISimulationRunFactory
{
    SimulationExecutionContext Create(
        ProcessConfiguration processConfiguration,
        SimulationRunOptions options);
}
```

Factory rules:

- wire run-scoped execution data once per run
- create the queue once per run and keep only the dequeue-facing `EventQueue` reference in the root context
- build handler-facing contexts from the same run-scoped state and stores
- provide scheduler, dispatcher, handlers, and similar runtime services through dependency injection instead of storing them inside the context object
- bootstrap scheduling of the first event through the injected scheduler used by the runtime bootstrapper

## Naming Rule

- `SimulationExecutionContext` is the live simulation-internal execution shell used by runner and dispatcher
- `SimulationExecutionState` is the technical cross-layer document used for checkpoint save, load, and orchestration flows
