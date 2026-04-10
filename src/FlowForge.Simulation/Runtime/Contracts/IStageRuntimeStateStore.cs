using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface IStageRuntimeStateStore
{
  public void Enqueue(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan currentTime);
  public StageQueueEntry? Dequeue(StageId stageId);

  public bool IsBusy(StageId stageId);

  public bool TryStartProcessing(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan startedAt,
    long processingToken);

  public bool TryFinishProcessing(
    StageId stageId,
    TrackingSubjectId trackingSubjectId);
}
