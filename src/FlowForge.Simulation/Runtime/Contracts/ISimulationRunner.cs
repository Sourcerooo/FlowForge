using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.Enums;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface ISimulationRunner
{
  public Task<SimulationRunResult> RunSimulation(SimulationExecutionContext context, CancellationToken cancellationToken);
}
