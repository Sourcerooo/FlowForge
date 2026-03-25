# Simulation Execution Context Design

This document is normative for run-scoped execution context, handler-facing context, and run factory direction.

## Execution Context Ownership

One simulation run owns one root execution context.
Handlers receive a narrower handler-facing context so the raw queue remains hidden.

Recommended owner and creation boundary:

- application orchestrates start
- simulation-side factory builds the full context once per run
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
    public ISimulationScheduler Scheduler { get; init; } = default!;
    public IEventDispatcher Dispatcher { get; init; } = default!;
    public IEventHandlerRegistry HandlerRegistry { get; init; } = default!;
    public IWorkItemTrackingStore WorkItemTrackingStore { get; init; } = default!;
    public IStationTrackingStore StationTrackingStore { get; init; } = default!;
    public IKpiCollector KpiCollector { get; init; } = default!;
    public ISnapshotBuilder SnapshotBuilder { get; init; } = default!;
    public ISnapshotStore SnapshotStore { get; init; } = default!;
    public ISnapshotTimelineStore SnapshotTimelineStore { get; init; } = default!;

    public SimulationExecutionHandlerContext CreateHandlerContext() => new()
    {
        SimulationRunId = SimulationRunId,
        ProcessConfiguration = ProcessConfiguration,
        Metadata = Metadata,
        State = State,
        Scheduler = Scheduler,
        WorkItemTrackingStore = WorkItemTrackingStore,
        StationTrackingStore = StationTrackingStore,
        KpiCollector = KpiCollector,
        SnapshotBuilder = SnapshotBuilder,
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
    public ISimulationScheduler Scheduler { get; init; } = default!;
    public IWorkItemTrackingStore WorkItemTrackingStore { get; init; } = default!;
    public IStationTrackingStore StationTrackingStore { get; init; } = default!;
    public IKpiCollector KpiCollector { get; init; } = default!;
    public ISnapshotBuilder SnapshotBuilder { get; init; } = default!;
    public ISnapshotStore SnapshotStore { get; init; } = default!;
    public ISnapshotTimelineStore SnapshotTimelineStore { get; init; } = default!;
}
```

Visibility rule:

- `SimulationExecutionContext` is for runner, dispatcher, and factory internals
- `SimulationExecutionHandlerContext` is the only context shape handed to event handlers
- this keeps dequeue access out of handler APIs even if one concrete queue object implements both queue and scheduler interfaces

## Context Interpretation

- `SimulationRunId` identifies one concrete execution and scopes queue entries, snapshots, and diagnostics
- `ProcessConfiguration` is the immutable process topology and arrival definition for the run
- `Metadata` holds run-scoped descriptive information that is not part of mutable execution state
- `State` holds mutable simulation state such as current simulated time, active work items, and station occupancy
- `EventQueue` is the run-local mutable priority queue and is runner-facing only
- `Scheduler` is the only write gateway into `EventQueue`
- `Dispatcher` resolves and invokes handlers for dequeued events
- `HandlerRegistry` is the immutable routing table built at startup
- tracking, KPI, and snapshot services are run-scoped collaborators used during execution and publication

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

- wire queue, scheduler, dispatcher, registry, tracking stores, KPI collector, and snapshot services once per run
- one concrete `SimulationQueue` may implement both `ISimulationEventQueue` and `ISimulationScheduler`
- the same concrete queue instance may be exposed through separate interface references
- only the root context keeps the dequeue-facing `EventQueue` reference
- handler-facing contexts keep only the scheduler reference
- the factory bootstraps the first scheduled event through `ISimulationScheduler`

## Naming Rule

- `SimulationExecutionContext` is the live simulation-internal execution shell used by runner and dispatcher
- `SimulationExecutionState` is the technical cross-layer document used for checkpoint save, load, and orchestration flows
