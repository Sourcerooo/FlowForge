using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Entities.Stages;
using FlowForge.Simulation.Tracking.Entities.Stations;
using static FlowForge.Simulation.Tracking.Entities.Stages.StageTracking;

namespace FlowForge.Simulation.Tracking.Services;

public sealed class StageTrackingStore(ProcessConfiguration ProcessConfiguration) : IStageTrackingStore
{
  private readonly Dictionary<StageId, StageTracking> _stageTrackings = ProcessConfiguration.Stages.ToDictionary(
    stageDefinition => stageDefinition.StageId,
    stageDefinition => new StageTracking(stageDefinition.StageId, stageDefinition.Stations.ToDictionary(
      station => station.StationId,
      station => new StationTracking(station.StationId, stageDefinition.StageId)
    ))
  );

  public Result<StageTracking> GetStageTracking(StageId stageId)
  {
    return _stageTrackings.TryGetValue(stageId, out var stageTracking)
      ? Result<StageTracking>.Success(stageTracking)
      : Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
  }

  public Result<StageTracking> EnqueueWorkItem(
    StageId stageId,
    StageEntry stageEntry,
    OnQueueOccurrence onQueueOccurrence)
  {
    if (!_stageTrackings.TryGetValue(stageId, out var stageTracking))
    {
      return Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
    }
    stageTracking.EnqueueWorkItem(stageEntry, onQueueOccurrence);
    return Result<StageTracking>.Success(stageTracking);
  }

  public Result<StageTracking> StartProcessingWorkItem(
    StageId stageId,
    StageEntry stageEntry,
    ProcessingKind entryKind)
  {
    if (!_stageTrackings.TryGetValue(stageId, out var stageTracking))
    {
      return Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
    }
    stageTracking.StartProcessingWorkItem(stageEntry, entryKind);
    return Result<StageTracking>.Success(stageTracking);
  }

  public Result<StageTracking> CompleteWorkItem(StageId stageId, StageEntry stageEntry)
  {
    if (!_stageTrackings.TryGetValue(stageId, out var stageTracking))
    {
      return Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
    }
    stageTracking.CompleteWorkItem(stageEntry);
    return Result<StageTracking>.Success(stageTracking);
  }


  public Result<StageTracking> PutOnHoldWorkItem(
    StageId stageId,
    StageEntry stageEntry,
    OnHoldOccurrence onHoldOccurrence)
  {
    if (!_stageTrackings.TryGetValue(stageId, out var stageTracking))
    {
      return Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
    }
    stageTracking.PutOnHoldWorkItem(stageEntry, onHoldOccurrence);
    return Result<StageTracking>.Success(stageTracking);
  }

  public Result<StageTracking> StopAndRequeueWorkItem(
    StageId stageId,
    StageEntry stageEntry,
    OnHoldOccurrence onHoldOccurrence)
  {
    if (!_stageTrackings.TryGetValue(stageId, out var stageTracking))
    {
      return Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
    }
    stageTracking.StopAndRequeueWorkItem(stageEntry, onHoldOccurrence);
    return Result<StageTracking>.Success(stageTracking);
  }

}
