using FlowForge.Simulation.Kpi.Contracts;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Scheduling.Contracts;
using FlowForge.Simulation.Snapshots.Contracts;
using FlowForge.Simulation.Tracking.Contracts;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationExecutionHandlerContext
{
  public SimulationRunId SimulationRunId { get; init; }
  public ProcessConfiguration ProcessConfiguration { get; init; } = default!;
  public SimulationMetadata Metadata { get; init; } = default!;
  public SimulationState State { get; init; } = default!;
  public ISimulationScheduler Scheduler { get; init; } = default!;
  public ITrackingSubjectStore TrackingSubjectStore { get; init; } = default!;
  public IWorkItemTrackingStore WorkItemTrackingStore { get; init; } = default!;
  public IStageTrackingStore StationTrackingStore { get; init; } = default!;
  public IKpiCollector KpiCollector { get; init; } = default!;
  public ISnapshotBuilder SnapshotBuilder { get; init; } = default!;
  public ISnapshotStore SnapshotStore { get; init; } = default!;
  public ISnapshotTimelineStore SnapshotTimelineStore { get; init; } = default!;
}
