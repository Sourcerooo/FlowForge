using FlowForge.Simulation.Events.SimulationEvents;

namespace FlowForge.Simulation.Scheduling.Contracts;

public interface ISimulationEventQueue
{
  public bool TryDequeue(out SimulationEvent nextEvent);
}
