using FlowForge.Domain.ProcessModel.ValueObjects;
using FlowForge.Simulation.Tracking.Enums;
using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Tracking.Entities.WorkItems;

public sealed record WorkItemTrackingSegment(
  TrackingSegmentType SegmentType,
  TimeSpan StartedAt,
  long ProcessingToken = 0,
  StageId? StageId = null,
  StationId? StationId = null,
  TimeSpan? EndedAt = null
  )
{
  public WorkItemTrackingSegmentId SegmentId { get; } = WorkItemTrackingSegmentId.NewId();
  public TimeSpan? Duration => EndedAt is null ? null : EndedAt.Value - StartedAt;

  public WorkItemTrackingSegment EndSegment(TimeSpan endedAt)
  {
    return endedAt < StartedAt
      ? throw new InvalidDataException("OrderTrackingSegment: EndDate is before StartDate")
      : EndedAt is null ? this with { EndedAt = endedAt } : this;
  }
}
