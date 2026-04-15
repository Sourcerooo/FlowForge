using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;
using static FlowForge.Simulation.Runtime.Entities.StageRuntimeState;

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

  public StageQueueEntry? Dequeue(StageId stageId)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return !found || stageRuntimeState == null
      ? null
      : stageRuntimeState.Dequeue();
  }
  public void Enqueue(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    if (found && stageRuntimeState != null)
    {
      stageRuntimeState.Enqueue(new StageQueueEntry(trackingSubjectId, currentTime));
    }
  }

  public bool IsBusy(StageId stageId)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return !found || stageRuntimeState == null || stageRuntimeState.IsBusy();
  }

  public void CompleteProcessing(StageId stageId, TrackingSubjectId trackingSubjectId)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    if (!found || stageRuntimeState == null)
    {
      throw new InvalidOperationException($"StageRuntimeStateStore->CompleteProcessing: TrackingSubjectId {trackingSubjectId} can't be completed. StageId {stageId} was not found or is invalid.");
    }
    stageRuntimeState.CompleteProcessing(trackingSubjectId);
  }
  public Result<StageStartedProcess> TryStartProcessing(
    StageId stageId,
    TimeSpan startedAt)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return !found || stageRuntimeState == null
      ? throw new InvalidOperationException($"StageRuntimeStateStore->CompleteProcessing: Process can't be started. StageId {stageId} was not found or is invalid.")
      : stageRuntimeState.TryStartProcessing(startedAt);
  }

  public void StopProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    if (!found || stageRuntimeState == null)
    {
      throw new InvalidOperationException($"StageRuntimeStateStore->StopProcessing: Process can't be started. StageId {stageId} was not found or is invalid.");
    }
    stageRuntimeState.StopProcessing(trackingSubjectId, currentTime);
  }
}
