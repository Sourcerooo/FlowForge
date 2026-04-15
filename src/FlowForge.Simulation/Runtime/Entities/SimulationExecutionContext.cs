using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationExecutionContext(
  SimulationRunId simulationRunId,
  SimulationExecutionContextData data
  )
{
  public SimulationRunId SimulationRunId { get; init; } = simulationRunId;
  public SimulationExecutionContextData Data { get; init; } = data;

  public SimulationExecutionHandlerContext CreateHandlerContext() => new()
  {
    SimulationRunId = SimulationRunId,
    ProcessConfiguration = Data.ProcessConfiguration,
    Metadata = Data.Metadata,
    State = Data.State,
    StageStore = new StageStore(Data.StageRuntimeStateStore, Data.StageTrackingStore),
    WorkItemStore = new WorkItemStore(Data.WorkItemRuntimeStateStore, Data.WorkItemTrackingStore),
    RoutingPolicy = Data.RoutingPolicy
  };

}
