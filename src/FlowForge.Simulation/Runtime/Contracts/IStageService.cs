using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.Entities;
using static FlowForge.Simulation.Runtime.Entities.StageRuntimeState;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface IStageService
{
  public Result<StageStartedProcess> TryStartProcessing(
    StageStore stageStore,
    StageId stageId,
    TimeSpan startedAt);

  public void StopProcessing(
    StageStore stageStore,
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);

  public void CompleteProcessing(
    StageStore stageStore,
    StageId stageId,
    TrackingSubjectId trackingSubjectId);

  public void Enqueue(
    StageStore stageStore,
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);
}
