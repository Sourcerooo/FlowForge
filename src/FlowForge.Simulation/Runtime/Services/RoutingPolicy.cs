using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Application.Contracts;

namespace FlowForge.Simulation.Runtime.Services;

internal class RoutingPolicy(ProcessConfiguration ProcessConfiguration) : IRoutingPolicy
{
  private readonly List<StageDefinition> _orderedStages = [.. ProcessConfiguration.Stages.OrderBy(stage => stage.Sequence)];
  public StageId? GetNextStage(StageId? currentStage)
  {
    if (currentStage == null)
    {
      var definition = _orderedStages.FirstOrDefault();
      return definition is null ? null : definition.StageId;
    }
    var currentIndex = _orderedStages.FindIndex(stage => stage.StageId == currentStage);
    return currentIndex == -1 || currentIndex == _orderedStages.Count - 1
      ? null
      : _orderedStages[currentIndex + 1].StageId;
  }
}
