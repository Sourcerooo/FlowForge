using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.Enums;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface ISimulationEngine
{
  public Task<SimulationRunResult> RunSimulationAsync(SimulationExecutionContext context, CancellationToken cancellationToken);
}
