using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.ValueObjects;
using static FlowForge.Simulation.Runtime.Entities.StageRuntimeState;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface IStageRuntimeStateStore
{
  public void Enqueue(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);
  public StageQueueEntry? Dequeue(StageId stageId);

  public bool IsBusy(StageId stageId);

  public Result<StageStartedProcess> TryStartProcessing(
    StageId stageId,
    TimeSpan startedAt);

  public void StopProcessing(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);

  public void CompleteProcessing(
    StageId stageId,
    TrackingSubjectId trackingSubjectId);
}
