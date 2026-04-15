using FlowForge.Domain.Process.Entities;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationExecutionContext(
  SimulationRunId simulationRunId,
  SimulationMetadata metaData,
  SimulationState state,
  ProcessConfiguration processConfiguration
  )
{
  public SimulationRunId SimulationRunId { get; init; } = simulationRunId;
  public ProcessConfiguration ProcessConfiguration { get; init; } = processConfiguration;
  public SimulationMetadata Metadata { get; init; } = metaData;
  public SimulationState State { get; init; } = state;

  public SimulationExecutionHandlerContext CreateHandlerContext() => new()
  {
    SimulationRunId = SimulationRunId,
    State = State
  };

}
