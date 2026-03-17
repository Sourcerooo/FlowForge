using FlowForge.Simulation.Events.SimulationEvents;

namespace FlowForge.Simulation.Scheduling.Contracts;

public interface ISimulationScheduler
{
  public void Schedule(SimulationEvent simulationEvent);
}
