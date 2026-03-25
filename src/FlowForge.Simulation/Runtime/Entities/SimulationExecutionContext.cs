using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationExecutionContext(
  SimulationRunId simulationRundId,
  SimulationExecutionContextData data
  )
{
  public SimulationRunId SimulationRunId { get; init; } = simulationRundId;
  public SimulationExecutionContextData Data { get; init; } = data;

  public SimulationExecutionHandlerContext CreateHandlerContext() => new()
  {
    SimulationRunId = SimulationRunId,
    ProcessConfiguration = Data.ProcessConfiguration,
    Metadata = Data.Metadata,
    State = Data.State,
    TrackingSubjectStore = Data.TrackingSubjectStore,
    WorkItemTrackingStore = Data.WorkItemTrackingStore,
    StageTrackingStore = Data.StageTrackingStore,
    SnapshotStore = Data.SnapshotStore,
    SnapshotTimelineStore = Data.SnapshotTimelineStore
  };

}
