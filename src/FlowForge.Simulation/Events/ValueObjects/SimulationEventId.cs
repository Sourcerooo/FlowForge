namespace FlowForge.Simulation.Events.ValueObjects;

public readonly record struct SimulationEventId(Guid Value)
{
  public static SimulationEventId NewId() => new SimulationEventId(Guid.NewGuid());
}
