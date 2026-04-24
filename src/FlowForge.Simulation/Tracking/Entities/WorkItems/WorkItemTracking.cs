using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.Enums;

namespace FlowForge.Simulation.Tracking.Entities.WorkItems;

public sealed class WorkItemTracking(
  TrackingSubjectId trackingSubjectId,
  TimeSpan createdAt,
  TimeSpan? completedAt = null
  )
{
  public TrackingSubjectId TrackingSubjectId { get; init; } = trackingSubjectId;
  public TimeSpan CreatedAt { get; init; } = createdAt;
  public TimeSpan? CompletedAt { get; private set; } = completedAt;
  public IReadOnlyList<WorkItemTrackingSegment> Segments => _segments;
  public TimeSpan? TotalLeadTime => CompletedAt is null ? null : CompletedAt.Value - CreatedAt;

  private readonly List<WorkItemTrackingSegment> _segments = new List<WorkItemTrackingSegment>();

  public void EnqueueWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime)
    => TransitionTo(workItem, TrackingSegmentType.QueueWait, currentTime);
  public void ProcessWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime)
    => TransitionTo(workItem, TrackingSegmentType.Processing, currentTime);
  public void StopWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime)
    => TransitionTo(workItem, TrackingSegmentType.OnHold, currentTime);

  public void CompleteProcessingWorkItem(TimeSpan currentTime)
  {
    EndLastSegment(currentTime);
  }

  public void CompleteWorkItem(TimeSpan completedAt)
  {
    EndLastSegment(completedAt);
    CompletedAt = completedAt;
  }
  public void SetCompletedAt(TimeSpan completionDate)
  {
    CompletedAt = completionDate;
  }

  private WorkItemTracking TransitionTo(
    WorkItemRuntimeState workItem,
    TrackingSegmentType targetSegmentType,
    TimeSpan currentTime
    )
  {
    if (_segments.Count > 0)
    {
      var currentSegment = _segments[^1];
      if (currentSegment.SegmentType == targetSegmentType)
      {
        return this;
      }
      _segments[^1] = currentSegment.EndSegment(currentTime);
      _segments.Add(StartSegment(
        currentSegment,
        targetSegmentType,
        currentTime)
      );
    }
    else
    {
      _segments.Add(StartSegment(
        targetSegmentType,
        currentTime,
        workItem.CurrentProcessingToken,
        workItem.CurrentStageId,
        workItem.CurrentStationId));
    }
    return this;
  }

  private static WorkItemTrackingSegment StartSegment(
  TrackingSegmentType segmentType,
  TimeSpan startedAt,
  ProcessingToken processingToken = default,
  StageId? stageId = null,
  StationId? stationId = null)
  {
    return new WorkItemTrackingSegment(
      segmentType,
      startedAt,
      processingToken,
      stageId,
      stationId,
      null);
  }

  private static WorkItemTrackingSegment StartSegment(
  WorkItemTrackingSegment prevSegment,
  TrackingSegmentType segmentType,
  TimeSpan startedAt)
  {
    return new WorkItemTrackingSegment(
      segmentType,
      startedAt,
      prevSegment.ProcessingToken,
      prevSegment.StageId,
      prevSegment.StationId,
      null);
  }

  private void EndLastSegment(TimeSpan currentTime)
  {
    if (_segments.Count > 0)
    {
      var currentSegment = _segments[^1];
      if (currentSegment.StartedAt > currentTime)
      {
        throw new InvalidOperationException($"WorkItemTracking for item: {TrackingSubjectId}. CompletionTime before StartTime");
      }
      _segments[^1] = currentSegment.EndSegment(currentTime);
    }
  }
  public override string? ToString() => $"TrackingSubjectId={TrackingSubjectId}, CreatedAt={CreatedAt}, CompletedAt={CompletedAt}, TotalLeadTime={TotalLeadTime}";
}
