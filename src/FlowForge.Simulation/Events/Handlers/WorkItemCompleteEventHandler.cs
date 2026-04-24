using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Application.ValueObjects;
using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Events.Handlers;

internal sealed class WorkItemCompleteEventHandler(
  IWorkItemProcessOrchestrator Orchestrator)
  : ISimulationEventHandler
{
  public EventKind CanHandle() => EventKind.WorkItemComplete;

  public async Task Process(
    SimulationEvent simulationEvent,
    SimulationExecutionHandlerContext context,
    CancellationToken cancellationToken)
  {
    var workItemCompleteEvent = (WorkItemCompleteEvent)simulationEvent;
    var curTime = context.State.CurrentTime;
    if (workItemCompleteEvent.TrackingSubjectId.Value == Guid.Empty)
    {
      throw new InvalidOperationException("TrackingSubjectId must be provided for WorkItemQueueEvent");
    }

    await Orchestrator.CompleteWorkItemAsync(
      new CompleteWorkItemCommand(
        workItemCompleteEvent.TrackingSubjectId,
        workItemCompleteEvent.ProcessingToken,
        new SimulationCommandContext(
          context.SimulationRunId,
          context.State)
        ), cancellationToken);

  }
}
