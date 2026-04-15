using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.Enums;

namespace FlowForge.Simulation.Tracking.Entities.WorkItems;

public sealed class WorkItemTracking(
  TrackingSubjectId trackingSubjectId,
  TimeSpan createdAt,
  WorkItemStatus currentStatus = WorkItemStatus.Created,
  StageId? currentStage = null,
  StationId? currentStation = null,
  ProcessingToken currentProcessingToken = default,
  TimeSpan? completedAt = null
  )
{
  public TrackingSubjectId TrackingSubjectId { get; init; } = trackingSubjectId;
  public TimeSpan CreatedAt { get; init; } = createdAt;
  public WorkItemStatus CurrentStatus { get; private set; } = currentStatus;
  public StageId? CurrentStage { get; private set; } = currentStage;
  public StationId? CurrentStation { get; private set; } = currentStation;
  public ProcessingToken CurrentProcessingToken { get; private set; } = currentProcessingToken;
  public TimeSpan? CompletedAt { get; private set; } = completedAt;
  public IReadOnlyList<WorkItemTrackingSegment> Segments => _segments;
  public TimeSpan? TotalLeadTime => CompletedAt is null ? null : CompletedAt.Value - CreatedAt;

  private readonly List<WorkItemTrackingSegment> _segments = new List<WorkItemTrackingSegment>();

  public void EnqueueWorkItem(TimeSpan currentTime)
    => TransitionTo(TrackingSegmentType.QueueWait, WorkItemStatus.InQueue, currentTime);
  public void ProcessWorkItem(TimeSpan currentTime)
    => TransitionTo(TrackingSegmentType.Processing, WorkItemStatus.Processing, currentTime);
  public void StopWorkItem(TimeSpan currentTime)
    => TransitionTo(TrackingSegmentType.OnHold, WorkItemStatus.OnHold, currentTime);
  public void CompleteWorkItem(TimeSpan completedAt)
  {
    if (_segments.Count > 0)
    {
      var currentSegment = _segments[^1];
      if (currentSegment.StartedAt > completedAt)
      {
        throw new InvalidOperationException($"WorkItemTracking for item: {TrackingSubjectId}. CompletionTime before StartTime");
      }
      _segments[^1] = currentSegment.EndSegment(completedAt);
    }
    CompletedAt = completedAt;
    CurrentStatus = WorkItemStatus.Completed;
  }

  public void SetCurrentStatus(WorkItemStatus status)
  {
    CurrentStatus = status;
  }

  public void SetCurrentStage(StageId? stageId)
  {
    CurrentStage = stageId;
  }

  public void SetCurrentStation(StationId? stationId)
  {
    CurrentStation = stationId;
  }

  public void SetCurrentProcessingToken(ProcessingToken processingToken)
  {
    CurrentProcessingToken = processingToken;
  }

  public void SetCompletedAt(TimeSpan completionDate)
  {
    CompletedAt = completionDate;
  }

  private WorkItemTracking TransitionTo(
    TrackingSegmentType targetSegmentType,
    WorkItemStatus targetStatus,
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
        CurrentProcessingToken,
        CurrentStage,
        CurrentStation));
    }
    CurrentStatus = targetStatus;
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

  public override string? ToString() => $"TrackingSubjectId={TrackingSubjectId}, CreatedAt={CreatedAt}, CurrentStatus={CurrentStatus}, CurrentStage={CurrentStage}, CurrentStation={CurrentStation}, CurrentProcessingToken={CurrentProcessingToken}, CompletedAt={CompletedAt}, TotalLeadTime={TotalLeadTime}";
}
