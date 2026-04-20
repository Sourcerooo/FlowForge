using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface IStageService
{
  public Result<StageEntry> TryStartProcessing(
    StageId stageId,
    TimeSpan startedAt);

  public Result<StageEntry> StopProcessing(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);

  public Result<StageEntry> CompleteProcessing(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);

  public Result<StageEntry> Enqueue(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);
}
