using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Services;

internal sealed class StageRuntimeStateStore(ProcessConfiguration ProcessConfiguration) : IStageRuntimeStateStore
{
  private readonly Dictionary<StageId, StageRuntimeState> _stageRuntimeStates = ProcessConfiguration.Stages.ToDictionary(
      stageDefinition => stageDefinition.StageId,
      stageDefinition => new StageRuntimeState(stageDefinition.StageId, stageDefinition.Stations.ToDictionary(
        station => station.StationId,
        station => new StationRuntimeState(station.StationId, stageDefinition.StageId, station.WorkerCount)
      ))
    );

  public StageEntry? Dequeue(StageId stageId)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return !found || stageRuntimeState == null
      ? null
      : stageRuntimeState.Dequeue();
  }
  public Result<StageEntry> Enqueue(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    if (found && stageRuntimeState != null)
    {
      var stageEntry = new StageEntry(trackingSubjectId, currentTime);
      stageRuntimeState.Enqueue(stageEntry);
      return Result<StageEntry>.Success(stageEntry);
    }
    return Result<StageEntry>.Failure(new InvalidOperationException($"StageRuntimeStateStore->Enqueue: Process can't be enqueued. StageId {stageId} was not found or is invalid."));
  }

  public bool IsBusy(StageId stageId)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return !found || stageRuntimeState == null || stageRuntimeState.IsBusy();
  }

  public Result<StageEntry> CompleteProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return !found || stageRuntimeState == null
      ? Result<StageEntry>.Failure(new InvalidOperationException($"StageRuntimeStateStore->CompleteProcessing: TrackingSubjectId {trackingSubjectId} can't be completed. StageId {stageId} was not found or is invalid."))
      : stageRuntimeState.CompleteProcessing(trackingSubjectId, currentTime);
  }
  public Result<StageEntry> TryStartProcessing(
    StageId stageId,
    TimeSpan startedAt)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return !found || stageRuntimeState == null
      ? Result<StageEntry>.Failure(new InvalidOperationException($"StageRuntimeStateStore->CompleteProcessing: Process can't be started. StageId {stageId} was not found or is invalid."))
      : stageRuntimeState.TryStartProcessing(startedAt);
  }

  public Result<StageEntry> StopAndRequeue(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return !found || stageRuntimeState == null
      ? Result<StageEntry>.Failure(new InvalidOperationException($"StageRuntimeStateStore->StopProcessing: Process can't be started. StageId {stageId} was not found or is invalid."))
      : stageRuntimeState.StopAndRequeue(trackingSubjectId, currentTime);
  }

  public Result<StageEntry> PutOnHold(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return !found || stageRuntimeState == null
      ? Result<StageEntry>.Failure(new InvalidOperationException($"StageRuntimeStateStore->PutOnHold: Process can't be put on hold. StageId {stageId} was not found or is invalid."))
      : stageRuntimeState.PutOnHold(trackingSubjectId, currentTime);
  }

  public Result<StageEntry> ResumeProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return !found || stageRuntimeState == null
      ? Result<StageEntry>.Failure(new InvalidOperationException($"StageRuntimeStateStore->ResumeProcessing: Process can't be resumed. StageId {stageId} was not found or is invalid."))
      : stageRuntimeState.ResumeProcessing(trackingSubjectId, currentTime);
  }
}
