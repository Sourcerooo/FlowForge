using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Tracking.Entities.Stages;
using static FlowForge.Simulation.Tracking.Entities.Stages.StageTracking;

namespace FlowForge.Simulation.Tracking.Contracts;

public interface IStageTrackingStore
{
  public Result<StageTracking> GetStageTracking(StageId stageId);

  public Result<StageTracking> EnqueueWorkItem(StageId stageId,
    OnQueueOccurrence onQueueOccurrence,
    TimeSpan processingTime = default);

  public Result<StageTracking> StartProcessingWorkItem(
    StageId stageId,
    ProcessingKind entryKind,
    TimeSpan queueWaitTime = default,
    TimeSpan onHoldTime = default);

  public Result<StageTracking> CompleteWorkItem(StageId stageId, TimeSpan processingTime);
  public Result<StageTracking> StopWorkItem(
    StageId stageId,
    TimeSpan processingTime, OnHoldOccurrence onHoldOccurrence);

  public Result<StageTracking> StopAndRequeueWorkItem(
    StageId stageId,
    TimeSpan processingTime, OnHoldOccurrence onHoldOccurrence);
}
