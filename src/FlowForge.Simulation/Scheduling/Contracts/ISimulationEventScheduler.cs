using FlowForge.Simulation.Events.SimulationEvents;

namespace FlowForge.Simulation.Scheduling.Contracts;

public interface ISimulationEventScheduler
{
  public void Schedule(SimulationEvent simulationEvent);
}
