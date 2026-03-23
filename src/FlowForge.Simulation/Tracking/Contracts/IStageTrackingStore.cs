using FlowForge.Domain.ProcessModel.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Tracking.Entities.Stages;

namespace FlowForge.Simulation.Tracking.Contracts;

public interface IStageTrackingStore
{
  public Result<StageTracking> GetStageTracking(StageId stageId);
}
