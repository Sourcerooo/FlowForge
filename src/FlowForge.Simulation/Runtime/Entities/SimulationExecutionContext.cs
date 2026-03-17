using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationExecutionContext(
  SimulationRunId simulationRundId,
  SimulationExecutionContextData data,
  SimulationExecutionContextService service
  )
{
  public SimulationRunId SimulationRunId { get; init; } = simulationRundId;
  public SimulationExecutionContextData Data { get; init; } = data;
  public SimulationExecutionContextService Service { get; init; } = service;

  public SimulationExecutionHandlerContext CreateHandlerContext() => new()
  {
    SimulationRunId = SimulationRunId,
    ProcessConfiguration = Data.ProcessConfiguration,
    Metadata = Data.Metadata,
    State = Data.State,
    Scheduler = Service.Scheduler,
    TrackingSubjectStore = Data.TrackingSubjectStore,
    WorkItemTrackingStore = Data.WorkItemTrackingStore,
    StationTrackingStore = Data.StationTrackingStore,
    KpiCollector = Service.KpiCollector,
    SnapshotBuilder = Service.SnapshotBuilder,
    SnapshotStore = Data.SnapshotStore,
    SnapshotTimelineStore = Data.SnapshotTimelineStore
  };

}
