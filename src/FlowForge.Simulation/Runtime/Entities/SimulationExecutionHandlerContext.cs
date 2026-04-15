using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationExecutionHandlerContext
{
  public SimulationRunId SimulationRunId { get; init; }
  public SimulationState State { get; init; } = default!;
}
