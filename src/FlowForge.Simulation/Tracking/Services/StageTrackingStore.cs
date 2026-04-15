using System.Collections.Concurrent;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Entities.Stages;

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
}
