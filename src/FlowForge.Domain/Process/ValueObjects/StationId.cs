namespace FlowForge.Domain.Process.ValueObjects;

public readonly record struct StationId(Guid Value)
{
  public static StationId NewId() => new(Guid.NewGuid());
  public override string ToString() => Value.ToString();
};
