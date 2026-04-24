using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Entities.WorkItems;
using Microsoft.Extensions.Logging;
namespace FlowForge.Simulation.Tracking.Services;

internal static partial class WorkItemTrackingStoreLog
{
  [LoggerMessage(
    EventId = 1001,
    Level = LogLevel.Information,
    Message = "Tracking item added: TrackingItemId={TrackingSubjectId}, " +
    "CreatedAt={CreatedAt}")]
  public static partial void TrackingItemAdded(
    ILogger<WorkItemTrackingStore> logger,
    TrackingSubjectId trackingSubjectId,
    TimeSpan createdAt);
}

public sealed class WorkItemTrackingStore(ILogger<WorkItemTrackingStore> logger) : IWorkItemTrackingStore
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
    TimeSpan? completedAt = null)
  {
    var tracking = new WorkItemTracking(
      trackingSubjectId, createdAt, completedAt);
    _workItemTrackings[trackingSubjectId] = tracking;
    WorkItemTrackingStoreLog.TrackingItemAdded(logger,
      tracking.TrackingSubjectId,
      tracking.CreatedAt);
    return tracking;
  }

  public Result EnqueueWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime)
  {
    if (!_workItemTrackings.TryGetValue(workItem.TrackingSubjectId, out WorkItemTracking? value))
    {
      return Result.Failure(new InvalidOperationException($"TrackingSubjectId {workItem.TrackingSubjectId} does not exist"));
    }
    value.EnqueueWorkItem(workItem, currentTime);
    return Result.Success();
  }

  public Result StartProcessingWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime)
  {
    if (!_workItemTrackings.TryGetValue(workItem.TrackingSubjectId, out WorkItemTracking? value))
    {
      return Result.Failure(new InvalidOperationException($"TrackingSubjectId {workItem.TrackingSubjectId} does not exist"));
    }
    value.ProcessWorkItem(workItem, currentTime);
    return Result.Success();
  }

  public Result StopProcessingWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime)
  {
    if (!_workItemTrackings.TryGetValue(workItem.TrackingSubjectId, out WorkItemTracking? value))
    {
      return Result.Failure(new InvalidOperationException($"TrackingSubjectId {workItem.TrackingSubjectId} does not exist"));
    }
    value.StopWorkItem(workItem, currentTime);
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

  public Result CompleteProcessingWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime)
  {
    if (!_workItemTrackings.TryGetValue(workItem.TrackingSubjectId, out WorkItemTracking? value))
    {
      return Result.Failure(new InvalidOperationException($"TrackingSubjectId {workItem.TrackingSubjectId} does not exist"));
    }
    value.CompleteProcessingWorkItem(currentTime);
    return Result.Success();
  }
}
