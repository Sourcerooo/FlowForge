using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface IStageRuntimeStateStore
{
  public Result<StageEntry> Enqueue(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);
  public StageEntry? Dequeue(StageId stageId);

  public bool IsBusy(StageId stageId);

  public Result<StageEntry> TryStartProcessing(
    StageId stageId,
    TimeSpan startedAt);

  public Result<StageEntry> StopAndRequeue(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);

  public Result<StageEntry> PutOnHold(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);

  public Result<StageEntry> ResumeProcessing(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);

  public Result<StageEntry> CompleteProcessing(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);
}
