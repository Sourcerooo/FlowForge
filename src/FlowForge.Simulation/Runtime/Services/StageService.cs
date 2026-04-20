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
  public Result<StageEntry> CompleteProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var result = StageRuntimeStateStore.CompleteProcessing(stageId, trackingSubjectId, currentTime);
    StageTrackingStore.CompleteWorkItem(stageId, result.Value.CompletedAt - result.Value.StartedAt);
    return result;
  }
  public Result<StageEntry> Enqueue(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var result = StageRuntimeStateStore.Enqueue(stageId, trackingSubjectId, currentTime);
    var occurrence = OnQueueOccurrence.First;
    var time = currentTime;
    StageTrackingStore.EnqueueWorkItem(stageId, occurrence, time);
    return result;
  }
  public Result<StageEntry> StopProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var result = StageRuntimeStateStore.StopProcessing(stageId, trackingSubjectId, currentTime);
    var onHoldOccurrence = OnHoldOccurrence.First;
    var processingTime = currentTime;
    StageTrackingStore.StopWorkItem(stageId, processingTime, onHoldOccurrence);
    return result;
  }
  public Result<StageEntry> TryStartProcessing(StageId stageId, TimeSpan startedAt)
  {
    var result = StageRuntimeStateStore.TryStartProcessing(stageId, startedAt);
    var entryKind = ProcessingKind.InitialStartFromQueue;
    var queueWaitTime = startedAt;
    var onHoldTime = startedAt;
    StageTrackingStore.StartProcessingWorkItem(stageId, entryKind, queueWaitTime, onHoldTime);
    return result;
  }
}
