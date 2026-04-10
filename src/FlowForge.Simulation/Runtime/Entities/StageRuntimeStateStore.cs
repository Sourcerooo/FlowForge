using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public record struct StationDefinitionDto(
  StationId StationId,
  int WorkerCapacity
);

public record struct StageDefinitionDto(
  StageId StageId,
  IReadOnlyList<StationDefinitionDto> Stations
);

internal sealed class StageRuntimeStateStore(IEnumerable<StageDefinitionDto> stageDefinitions) : IStageRuntimeStateStore
{
  private readonly Dictionary<StageId, StageRuntimeState> _stageRuntimeStates = stageDefinitions.ToDictionary(
      stageDefinition => stageDefinition.StageId,
      stageDefinition => new StageRuntimeState(stageDefinition.StageId, stageDefinition.Stations.ToDictionary(
        station => station.StationId,
        station => new StationRuntimeState(station.StationId, stageDefinition.StageId, station.WorkerCapacity)
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

  public bool TryFinishProcessing(StageId stageId, TrackingSubjectId trackingSubjectId)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return found
      && stageRuntimeState != null
      && stageRuntimeState.TryFinishProcessing(trackingSubjectId);
  }
  public bool TryStartProcessing(
    StageId stageId,
    TrackingSubjectId trackingSubjectId,
    TimeSpan startedAt,
    long processingToken)
  {
    var found = _stageRuntimeStates.TryGetValue(stageId, out var stageRuntimeState);
    return found
      && stageRuntimeState != null
      && stageRuntimeState.TryStartProcessing(trackingSubjectId, startedAt, processingToken);
  }
}
