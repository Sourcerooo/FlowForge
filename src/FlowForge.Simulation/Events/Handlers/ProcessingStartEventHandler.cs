using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Application.ValueObjects;
using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Events.Handlers;

internal sealed class ProcessingStartEventHandler(
  IWorkItemProcessOrchestrator Orchestrator)
  : ISimulationEventHandler
{
  public EventKind CanHandle() => EventKind.ProcessingStart;

  public async Task Process(
    SimulationEvent simulationEvent,
    SimulationExecutionHandlerContext context,
    CancellationToken cancellationToken)
  {
    var processingStartEvent = (ProcessingStartEvent)simulationEvent;
    var curTime = context.State.CurrentTime;

    await Orchestrator.StartProcessingAsync(
      new StartProcessingCommand(
        processingStartEvent.StageId,
        new SimulationCommandContext(
          context.SimulationRunId,
          context.State)
        ), cancellationToken);

  }
}
