namespace FlowForge.Domain.Process.ValueObjects;

public readonly record struct StageId(Guid Value)
{
  public static StageId NewId() => new(Guid.NewGuid());
  public override string ToString() => Value.ToString();
};
