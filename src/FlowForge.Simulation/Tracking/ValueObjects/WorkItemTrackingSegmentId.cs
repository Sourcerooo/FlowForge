namespace FlowForge.Simulation.Tracking.ValueObjects;

public readonly record struct WorkItemTrackingSegmentId(Guid Value)
{
  public static WorkItemTrackingSegmentId NewId() => new WorkItemTrackingSegmentId(Guid.NewGuid());
  public static readonly WorkItemTrackingSegmentId Empty = new WorkItemTrackingSegmentId(Guid.Empty);
}
