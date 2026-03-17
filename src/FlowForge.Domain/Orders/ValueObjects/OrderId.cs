namespace FlowForge.Domain.Orders.ValueObjects;

public readonly record struct OrderId(Guid Value)
{
  public static OrderId NewId() => new(Guid.NewGuid());
};
