namespace FlowForge.Simulation.Tracking.ValueObjects;

public readonly record struct TrackingSubjectId(Guid Value)
{
  public static TrackingSubjectId NewId() => new TrackingSubjectId(Guid.NewGuid());
}
