using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Tracking.Enums;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class WorkItemRuntimeState(
  TrackingSubjectId trackingSubjectId,
  TimeSpan createdAt,
  WorkItemStatus currentStatus = WorkItemStatus.Created,
  StageId? currentStageId = null,
  StationId? currentStationId = null,
  long currentProcessingToken = 0,
  TimeSpan? completedAt = null)
{
  public TrackingSubjectId TrackingSubjectId { get; } = trackingSubjectId;
  public TimeSpan CreatedAt { get; } = createdAt;
  public WorkItemStatus CurrentStatus { get; private set; } = currentStatus;
  public StageId? CurrentStageId { get; private set; } = currentStageId;
  public StationId? CurrentStationId { get; private set; } = currentStationId;
  public long CurrentProcessingToken { get; private set; } = currentProcessingToken;
  public TimeSpan? CompletedAt { get; private set; } = completedAt;

  public void QueueForStage(StageId stageId, long processingToken = 0)
  {
    CurrentStatus = WorkItemStatus.InQueue;
    CurrentStageId = stageId;
    CurrentStationId = null;
    CurrentProcessingToken = processingToken;
  }

  public void StartProcessing(StationId stationId)
  {
    CurrentStatus = WorkItemStatus.Processing;
    CurrentStationId = stationId;
  }

  public void CompleteProcessing()
  {
    CurrentStatus = WorkItemStatus.Completed;
  }

  public void StopProcessing()
  {
    CurrentStatus = WorkItemStatus.OnHold;
  }

  public void CompleteWorkItem(TimeSpan completedAt)
  {
    CurrentStatus = WorkItemStatus.Finished;
    CompletedAt = completedAt;
  }
}


