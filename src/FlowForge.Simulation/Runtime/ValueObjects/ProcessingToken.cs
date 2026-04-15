namespace FlowForge.Simulation.Runtime.ValueObjects;

public readonly record struct ProcessingToken(long Value)
{
  public static ProcessingToken Initial => new(0);
}

