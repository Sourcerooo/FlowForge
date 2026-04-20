using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Application.ValueObjects;
using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Events.Handlers;

internal sealed class ProcessingCompleteEventHandler(
  IWorkItemProcessOrchestrator Orchestrator)
  : ISimulationEventHandler
{
  public EventKind CanHandle() => EventKind.ProcessingComplete;

  public async Task Process(
    SimulationEvent simulationEvent,
    SimulationExecutionHandlerContext context,
    CancellationToken cancellationToken)
  {
    var processingCompleteEvent = (ProcessingCompleteEvent)simulationEvent;
    var curTime = context.State.CurrentTime;
    if (processingCompleteEvent.TrackingSubjectId.Value == Guid.Empty)
    {
      throw new InvalidOperationException("TrackingSubjectId must be provided for WorkItemQueueEvent");
    }

    await Orchestrator.CompleteProcessingAsync(
      new CompleteProcessingCommand(
        processingCompleteEvent.TrackingSubjectId,
        processingCompleteEvent.ProcessingToken,
        processingCompleteEvent.StageId,
        new SimulationCommandContext(
          context.SimulationRunId,
          context.State)
        ), cancellationToken);

  }
}
