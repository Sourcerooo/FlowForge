using FlowForge.Domain.Process.Entities;
using FlowForge.Simulation.Runtime.Entities;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface ISimulationContextBuilder
{
  public SimulationExecutionContext Build(ProcessConfiguration processConfiguration);
}
