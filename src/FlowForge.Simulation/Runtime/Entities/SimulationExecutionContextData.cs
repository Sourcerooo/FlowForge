using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Scheduling.Contracts;
using FlowForge.Simulation.Snapshots.Contracts;
using FlowForge.Simulation.Tracking.Contracts;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationExecutionContextData
{
  public ProcessConfiguration ProcessConfiguration { get; init; } = default!;
  public SimulationMetadata Metadata { get; init; } = default!;
  public SimulationState State { get; init; } = default!;
  public ISimulationEventQueue EventQueue { get; init; } = default!;

  public ITrackingSubjectStore TrackingSubjectStore { get; init; } = default!;
  public IWorkItemTrackingStore WorkItemTrackingStore { get; init; } = default!;
  public IStageTrackingStore StageTrackingStore { get; init; } = default!;
  public ISnapshotStore SnapshotStore { get; init; } = default!;
  public ISnapshotTimelineStore SnapshotTimelineStore { get; init; } = default!;
}
