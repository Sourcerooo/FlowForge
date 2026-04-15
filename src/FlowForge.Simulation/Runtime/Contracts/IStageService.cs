using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using static FlowForge.Simulation.Runtime.Entities.StageRuntimeState;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface IStageService
{
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

  public void Enqueue(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);
}
