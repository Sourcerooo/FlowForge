using FlowForge.Simulation.Events.ValueObjects;

namespace FlowForge.Simulation.Events.Contracts;

public interface IEventHandlerRegistry
{
  public void Register(EventRoutingKey key, ISimulationEventHandler handler);
  public ISimulationEventHandler Resolve(EventRoutingKey key);
}
