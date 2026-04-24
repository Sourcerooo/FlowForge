using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Entities.Stages;
using static FlowForge.Simulation.Tracking.Entities.Stages.StageTracking;

namespace FlowForge.Simulation.Runtime.Services;

internal class StageService(IStageRuntimeStateStore StageRuntimeStateStore, IStageTrackingStore StageTrackingStore) : IStageService
{
  private readonly HashSet<(StageId StageId, TrackingSubjectId TrackingSubjectId)> _workItemsSeenOnHold = [];

  public Result<StageEntry> CompleteProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var result = StageRuntimeStateStore.CompleteProcessing(stageId, trackingSubjectId, currentTime);
    if (result.IsSuccess)
    {
      StageTrackingStore.CompleteWorkItem(stageId, result.Value);
    }
    return result;
  }
  public Result<StageEntry> Enqueue(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var result = StageRuntimeStateStore.Enqueue(stageId, trackingSubjectId, currentTime);
    if (result.IsSuccess)
    {
      var occurrence = result.Value.RequeuedAt == default
        ? OnQueueOccurrence.First
        : OnQueueOccurrence.Requeued;
      StageTrackingStore.EnqueueWorkItem(stageId, result.Value, occurrence);
    }
    return result;
  }
  public Result<StageEntry> StopAndRequeue(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var result = StageRuntimeStateStore.StopAndRequeue(stageId, trackingSubjectId, currentTime);
    if (result.IsSuccess)
    {
      var onHoldOccurrence = GetOnHoldOccurrence(stageId, trackingSubjectId);
      StageTrackingStore.StopAndRequeueWorkItem(stageId, result.Value, onHoldOccurrence);
    }
    return result;
  }

  public Result<StageEntry> PutOnHold(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var result = StageRuntimeStateStore.PutOnHold(stageId, trackingSubjectId, currentTime);
    if (result.IsSuccess)
    {
      StageTrackingStore.PutOnHoldWorkItem(stageId, result.Value, GetOnHoldOccurrence(stageId, trackingSubjectId));
    }

    return result;
  }

  public Result<StageEntry> ResumeProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var result = StageRuntimeStateStore.ResumeProcessing(stageId, trackingSubjectId, currentTime);
    if (result.IsSuccess)
    {
      StageTrackingStore.StartProcessingWorkItem(stageId, result.Value, ProcessingKind.ResumeFromOnHold);
    }

    return result;
  }

  public Result<StageEntry> TryStartProcessing(StageId stageId, TimeSpan startedAt)
  {
    var result = StageRuntimeStateStore.TryStartProcessing(stageId, startedAt);
    if (result.IsSuccess)
    {
      var entryKind = result.Value.RequeuedAt == default
        ? ProcessingKind.InitialStartFromQueue
        : ProcessingKind.ResumeFromQueue;
      StageTrackingStore.StartProcessingWorkItem(stageId, result.Value, entryKind);
    }
    return result;
  }

  private OnHoldOccurrence GetOnHoldOccurrence(StageId stageId, TrackingSubjectId trackingSubjectId)
  {
    return _workItemsSeenOnHold.Add((stageId, trackingSubjectId))
      ? OnHoldOccurrence.First
      : OnHoldOccurrence.Repeated;
  }
}
