using System.Collections.Concurrent;
using FlowForge.Domain.ProcessModel.ValueObjects;
using FlowForge.Simulation.Tracking.Contracts;

namespace FlowForge.Simulation.Tracking.Entities.Stages;

public sealed class StageTrackingStore : IStageTrackingStore
{
  private readonly ConcurrentDictionary<StageId, StageTracking> _stageTrackings
  = new ConcurrentDictionary<StageId, StageTracking>();
}
