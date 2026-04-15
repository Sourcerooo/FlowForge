using FlowForge.Domain.Process.Entities;
using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationExecutionHandlerContext
{
  public SimulationRunId SimulationRunId { get; init; }
  public ProcessConfiguration ProcessConfiguration { get; init; } = default!;
  public SimulationMetadata Metadata { get; init; } = default!;
  public SimulationState State { get; init; } = default!;
  public StageStore StageStore { get; init; } = default!;
  public WorkItemStore WorkItemStore { get; init; } = default!;
  public IRoutingPolicy RoutingPolicy { get; init; } = default!;
}
