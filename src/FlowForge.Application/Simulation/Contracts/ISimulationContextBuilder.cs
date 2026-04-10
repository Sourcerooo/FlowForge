using FlowForge.Simulation.Runtime.Entities;

namespace FlowForge.Application.Simulation.Contracts;

public interface ISimulationContextBuilder
{
  public SimulationExecutionContext Build();
}
