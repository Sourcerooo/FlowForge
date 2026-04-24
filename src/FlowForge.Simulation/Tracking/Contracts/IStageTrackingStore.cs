using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.Entities.Stages;
using static FlowForge.Simulation.Tracking.Entities.Stages.StageTracking;

namespace FlowForge.Simulation.Tracking.Contracts;

public interface IStageTrackingStore
{
  public Result<StageTracking> GetStageTracking(StageId stageId);

  public Result<StageTracking> EnqueueWorkItem(StageId stageId,
    StageEntry stageEntry,
    OnQueueOccurrence onQueueOccurrence);

  public Result<StageTracking> StartProcessingWorkItem(
    StageId stageId,
    StageEntry stageEntry,
    ProcessingKind entryKind);

  public Result<StageTracking> CompleteWorkItem(StageId stageId, StageEntry stageEntry);
  public Result<StageTracking> PutOnHoldWorkItem(
    StageId stageId,
    StageEntry stageEntry,
    OnHoldOccurrence onHoldOccurrence);

  public Result<StageTracking> StopAndRequeueWorkItem(
    StageId stageId,
    StageEntry stageEntry,
    OnHoldOccurrence onHoldOccurrence);
}
