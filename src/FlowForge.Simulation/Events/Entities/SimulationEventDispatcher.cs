using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Runtime.Entities;

namespace FlowForge.Simulation.Events.Entities;

internal sealed class SimulationEventDispatcher(IEnumerable<ISimulationEventHandler> eventHandler) : ISimulationEventDispatcher
{
  private readonly Dictionary<EventKind, ISimulationEventHandler> _eventHandler
    = eventHandler.ToDictionary<ISimulationEventHandler, EventKind>(simulationEventHandler => simulationEventHandler.CanHandle());

  public async Task DispatchAsync(
    SimulationEvent simulationEvent,
    SimulationExecutionHandlerContext context,
    CancellationToken cancellationToken)
  {
    //StageId? stageId = null;
    var handler = _eventHandler.GetValueOrDefault(simulationEvent.EventKind);
    if (handler != null)
    {
      await handler.Process(simulationEvent, context, cancellationToken);
    }
  }
}
