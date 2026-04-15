using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Application.Contracts;

namespace FlowForge.Simulation.Runtime.Services;

internal class RoutingPolicy : IRoutingPolicy
{
  public StageId? GetNextStage(StageId? currentStage)
  {
    return null;
  }
}
