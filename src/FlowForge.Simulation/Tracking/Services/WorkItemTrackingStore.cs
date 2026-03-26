using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Entities.WorkItems;
using FlowForge.Simulation.Tracking.Enums;
using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Tracking.Services;

public sealed class WorkItemTrackingStore : IWorkItemTrackingStore
{
  private readonly Dictionary<TrackingSubjectId, WorkItemTracking> _workItemTrackings
    = new Dictionary<TrackingSubjectId, WorkItemTracking>();

  public Result<WorkItemTracking> GetWorkItemTracking(TrackingSubjectId trackingSubjectId)
  {
    return _workItemTrackings.TryGetValue(trackingSubjectId, out var tracking)
      ? Result<WorkItemTracking>.Success(tracking)
      : Result<WorkItemTracking>.Failure(new InvalidOperationException($"TrackingSubjectId {trackingSubjectId} does not exist"));
  }

  public WorkItemTracking AddWorkItemTracking(
    TrackingSubjectId trackingSubjectId,
    TimeSpan createdAt,
    WorkItemStatus currentStatus = WorkItemStatus.Created,
    StageId? currentStageId = null,
    StationId? currentStationId = null,
    long currentProcessingToken = 0,
    TimeSpan? completedAt = null)
  {
    var tracking = new WorkItemTracking(
      trackingSubjectId, createdAt, currentStatus, currentStageId, currentStationId,
      currentProcessingToken, completedAt);
    _workItemTrackings[trackingSubjectId] = tracking;
    return tracking;
  }

  public Result SetCurrentStatus(TrackingSubjectId trackingSubjectId, WorkItemStatus status)
  {
    var trackingResult = GetWorkItemTracking(trackingSubjectId);
    if (trackingResult.IsFailure)
    {
      return Result.Failure(trackingResult.Exception!);
    }
    trackingResult.Value?.SetCurrentStatus(status);
    return Result.Success();
  }
  public Result SetCurrentStageId(TrackingSubjectId trackingSubjectId, StageId? stageId)
  {
    var trackingResult = GetWorkItemTracking(trackingSubjectId);
    if (trackingResult.IsFailure)
    {
      return Result.Failure(trackingResult.Exception!);
    }
    trackingResult.Value?.SetCurrentStage(stageId);
    trackingResult.Value?.SetCurrentStation(null);
    return Result.Success();
  }

  public Result SetCurrentStationId(TrackingSubjectId trackingSubjectId, StageId? stageId, StationId? stationId)
  {
    var trackingResult = GetWorkItemTracking(trackingSubjectId);
    if (trackingResult.IsFailure)
    {
      return Result.Failure(trackingResult.Exception!);
    }
    trackingResult.Value?.SetCurrentStage(stageId);
    trackingResult.Value?.SetCurrentStation(stationId);
    return Result.Success();
  }

  public Result SetCurrentProcessingToken(TrackingSubjectId trackingSubjectId, long processingToken)
  {
    var trackingResult = GetWorkItemTracking(trackingSubjectId);
    if (trackingResult.IsFailure)
    {
      return Result.Failure(trackingResult.Exception!);
    }
    trackingResult.Value?.SetCurrentProcessingToken(processingToken);
    return Result.Success();
  }

  public Result EnqueueWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    if (!_workItemTrackings.TryGetValue(trackingSubjectId, out WorkItemTracking? value))
    {
      return Result.Failure(new InvalidOperationException($"TrackingSubjectId {trackingSubjectId} does not exist"));
    }
    value.EnqueueWorkItem(currentTime);
    return Result.Success();
  }

  public Result ProcessWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    if (!_workItemTrackings.TryGetValue(trackingSubjectId, out WorkItemTracking? value))
    {
      return Result.Failure(new InvalidOperationException($"TrackingSubjectId {trackingSubjectId} does not exist"));
    }
    value.ProcessWorkItem(currentTime);
    return Result.Success();
  }

  public Result StopWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    if (!_workItemTrackings.TryGetValue(trackingSubjectId, out WorkItemTracking? value))
    {
      return Result.Failure(new InvalidOperationException($"TrackingSubjectId {trackingSubjectId} does not exist"));
    }
    value.StopWorkItem(currentTime);
    return Result.Success();
  }

  public Result CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan completionTime)
  {
    if (!_workItemTrackings.TryGetValue(trackingSubjectId, out WorkItemTracking? value))
    {
      return Result.Failure(new InvalidOperationException($"TrackingSubjectId {trackingSubjectId} does not exist"));
    }
    value.CompleteWorkItem(completionTime);
    return Result.Success();
  }

}
