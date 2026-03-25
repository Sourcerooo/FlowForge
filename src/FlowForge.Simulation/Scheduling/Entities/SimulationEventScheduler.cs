using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Scheduling.Contracts;

namespace FlowForge.Simulation.Scheduling.Entities;

internal sealed class SimulationEventScheduler(ISimulationEventQueue EventQueue) : ISimulationEventScheduler
{
  public void Schedule(SimulationEvent simulationEvent)
  {
    EventQueue.Queue(simulationEvent);
  }
}
