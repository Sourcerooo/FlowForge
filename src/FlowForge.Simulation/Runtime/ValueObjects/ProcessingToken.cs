namespace FlowForge.Simulation.Runtime.ValueObjects;

internal readonly record struct ProcessingToken(long Value)
{
  public static ProcessingToken Initial => new(0);
}

