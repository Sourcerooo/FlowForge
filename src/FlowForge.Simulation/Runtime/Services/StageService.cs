using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using static FlowForge.Simulation.Runtime.Entities.StageRuntimeState;

namespace FlowForge.Simulation.Runtime.Services;

internal class StageService() : IStageService
{
  public void CompleteProcessing(StageStore stageStore, StageId stageId, TrackingSubjectId trackingSubjectId)
  {
    stageStore.StageRuntimeStore.CompleteProcessing(stageId, trackingSubjectId);
  }
  public void Enqueue(StageStore stageStore, StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    stageStore.StageRuntimeStore.Enqueue(stageId, trackingSubjectId, currentTime);
  }
  public void StopProcessing(StageStore stageStore, StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    stageStore.StageRuntimeStore.StopProcessing(stageId, trackingSubjectId, currentTime);
  }
  public Result<StageStartedProcess> TryStartProcessing(StageStore stageStore, StageId stageId, TimeSpan startedAt)
  {
    return stageStore.StageRuntimeStore.TryStartProcessing(stageId, startedAt);
  }
}
