using FlowForge.Domain.Process.Entities;
using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Application.Entities;

namespace FlowForge.Simulation.Application.Services;

public class SimulationScopeFactory(IServiceProvider RootServiceProvider) : ISimulationScopeFactory
{
  public SimulationScope CreateScope(ProcessConfiguration processConfiguration)
  {
    ArgumentNullException.ThrowIfNull(processConfiguration);
    return SimulationScope.Create(processConfiguration, RootServiceProvider);
  }
}
