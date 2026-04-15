namespace FlowForge.Simulation.Runtime.ValueObjects;

public readonly record struct SimulationRunId(Guid Value)
{
  public static SimulationRunId NewId() => new(Guid.NewGuid());
  public override string ToString() => Value.ToString();
};
