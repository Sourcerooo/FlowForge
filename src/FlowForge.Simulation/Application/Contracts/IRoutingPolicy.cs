using FlowForge.Domain.Process.ValueObjects;

namespace FlowForge.Simulation.Application.Contracts;

public interface IRoutingPolicy
{
  public StageId? GetNextStage(StageId? currentStage);
}
