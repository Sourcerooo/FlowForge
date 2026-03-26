using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Tracking.Entities.WorkItems;
using FlowForge.Simulation.Tracking.Enums;
using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Tracking.Contracts;

public interface IWorkItemTrackingStore
{
  public Result<WorkItemTracking> GetWorkItemTracking(
    TrackingSubjectId trackingSubjectId);

  public WorkItemTracking AddWorkItemTracking(
    TrackingSubjectId trackingSubjectId,
    TimeSpan createdAt,
    WorkItemStatus currentStatus = WorkItemStatus.Created,
    StageId? currentStageId = null,
    StationId? currentStationId = null,
    long currentProcessingToken = 0,
    TimeSpan? completedAt = null);
  public Result SetCurrentProcessingToken(TrackingSubjectId trackingSubjectId, long processingToken);
  public Result SetCurrentStageId(TrackingSubjectId trackingSubjectId, StageId? stageId);
  public Result SetCurrentStationId(TrackingSubjectId trackingSubjectId, StageId? stageId, StationId? stationId);
  public Result SetCurrentStatus(TrackingSubjectId trackingSubjectId, WorkItemStatus status);

  public Result EnqueueWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
  public Result ProcessWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
  public Result StopWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
  public Result CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan completionTime);
}
