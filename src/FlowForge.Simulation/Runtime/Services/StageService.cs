using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Tracking.Contracts;
using static FlowForge.Simulation.Runtime.Entities.StageRuntimeState;

namespace FlowForge.Simulation.Runtime.Services;

internal class StageService(IStageRuntimeStateStore StageRuntimeStateStore, IStageTrackingStore StageTrackingStore) : IStageService
{
  public void CompleteProcessing(StageId stageId, TrackingSubjectId trackingSubjectId)
  {
    StageRuntimeStateStore.CompleteProcessing(stageId, trackingSubjectId);
  }
  public void Enqueue(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    StageRuntimeStateStore.Enqueue(stageId, trackingSubjectId, currentTime);
  }
  public void StopProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    StageRuntimeStateStore.StopProcessing(stageId, trackingSubjectId, currentTime);
  }
  public Result<StageStartedProcess> TryStartProcessing(StageId stageId, TimeSpan startedAt)
  {
    return StageRuntimeStateStore.TryStartProcessing(stageId, startedAt);
  }
}
