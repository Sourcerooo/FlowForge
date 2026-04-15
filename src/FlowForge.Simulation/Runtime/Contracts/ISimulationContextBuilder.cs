using FlowForge.Simulation.Runtime.Entities;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface ISimulationContextBuilder
{
  public SimulationExecutionContext Build();
}
