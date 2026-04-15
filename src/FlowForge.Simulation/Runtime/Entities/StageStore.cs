using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Tracking.Contracts;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class StageStore(
  IStageRuntimeStateStore stageRuntimeStore,
  IStageTrackingStore stageTrackingStore)
{
  public IStageRuntimeStateStore StageRuntimeStore { get; } = stageRuntimeStore;
  public IStageTrackingStore StageTrackingStore { get; } = stageTrackingStore;
};
