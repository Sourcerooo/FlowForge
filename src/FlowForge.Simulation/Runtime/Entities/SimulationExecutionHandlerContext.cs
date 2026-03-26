using FlowForge.Domain.Process.Entities;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationExecutionHandlerContext
{
  public SimulationRunId SimulationRunId { get; init; }
  public ProcessConfiguration ProcessConfiguration { get; init; } = default!;
  public SimulationMetadata Metadata { get; init; } = default!;
  public SimulationState State { get; init; } = default!;
}
