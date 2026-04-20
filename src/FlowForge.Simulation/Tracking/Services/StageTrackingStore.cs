using System.Collections.Concurrent;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Entities.Stages;
using static FlowForge.Simulation.Tracking.Entities.Stages.StageTracking;

namespace FlowForge.Simulation.Tracking.Services;

public sealed class StageTrackingStore : IStageTrackingStore
{
  private readonly ConcurrentDictionary<StageId, StageTracking> _stageTrackings
  = new ConcurrentDictionary<StageId, StageTracking>();

  public Result<StageTracking> GetStageTracking(StageId stageId)
  {
    return _stageTrackings.TryGetValue(stageId, out var stageTracking)
      ? Result<StageTracking>.Success(stageTracking)
      : Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
  }

  public Result<StageTracking> EnqueueWorkItem(
    StageId stageId,
    OnQueueOccurrence onQueueOccurrence,
    TimeSpan processingTime = default
    )
  {
    if (!_stageTrackings.TryGetValue(stageId, out var stageTracking))
    {
      return Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
    }
    stageTracking.EnqueueWorkItem(onQueueOccurrence, processingTime);
    return Result<StageTracking>.Success(stageTracking);
  }

  public Result<StageTracking> StartProcessingWorkItem(
    StageId stageId,
    ProcessingKind entryKind,
    TimeSpan queueWaitTime = default,
    TimeSpan onHoldTime = default)
  {
    if (!_stageTrackings.TryGetValue(stageId, out var stageTracking))
    {
      return Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
    }
    stageTracking.StartProcessingWorkItem(entryKind, queueWaitTime, onHoldTime);
    return Result<StageTracking>.Success(stageTracking);
  }

  public Result<StageTracking> CompleteWorkItem(StageId stageId, TimeSpan processingTime)
  {
    if (!_stageTrackings.TryGetValue(stageId, out var stageTracking))
    {
      return Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
    }
    stageTracking.CompleteWorkItem(processingTime);
    return Result<StageTracking>.Success(stageTracking);
  }


  public Result<StageTracking> StopWorkItem(
    StageId stageId,
    TimeSpan processingTime, OnHoldOccurrence onHoldOccurrence)
  {
    if (!_stageTrackings.TryGetValue(stageId, out var stageTracking))
    {
      return Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
    }
    stageTracking.StopWorkItem(processingTime, onHoldOccurrence);
    return Result<StageTracking>.Success(stageTracking);
  }

  public Result<StageTracking> StopAndRequeueWorkItem(
    StageId stageId,
    TimeSpan processingTime, OnHoldOccurrence onHoldOccurrence)
  {
    if (!_stageTrackings.TryGetValue(stageId, out var stageTracking))
    {
      return Result<StageTracking>.Failure(new InvalidDataException($"Tracking with StageId {stageId} does not exist"));
    }
    stageTracking.StopWorkItem(processingTime, onHoldOccurrence);
    return Result<StageTracking>.Success(stageTracking);
  }

}
