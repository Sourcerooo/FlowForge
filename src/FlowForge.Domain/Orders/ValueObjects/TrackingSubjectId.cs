namespace FlowForge.Domain.Orders.ValueObjects;

public readonly record struct TrackingSubjectId(Guid Value)
{
  public static TrackingSubjectId NewId() => new(Guid.NewGuid());
};
