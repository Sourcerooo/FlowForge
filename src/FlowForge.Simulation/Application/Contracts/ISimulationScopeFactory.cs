using FlowForge.Domain.Process.Entities;
using FlowForge.Simulation.Application.Entities;

namespace FlowForge.Simulation.Application.Contracts;

public interface ISimulationScopeFactory
{
  public SimulationScope CreateScope(ProcessConfiguration processConfiguration);
}
