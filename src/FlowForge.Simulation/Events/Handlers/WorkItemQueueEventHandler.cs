using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Application.ValueObjects;
using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Events.Handlers;

internal sealed class WorkItemQueueEventHandler(
  IWorkItemProcessOrchestrator Orchestrator)
  : ISimulationEventHandler
{
  public EventKind CanHandle() => EventKind.WorkItemQueue;

  public async Task Process(
    SimulationEvent simulationEvent,
    SimulationExecutionHandlerContext context,
    CancellationToken cancellationToken)
  {
    var workItemQueueEvent = (WorkItemQueueEvent)simulationEvent;
    var curTime = context.State.CurrentTime;
    if (workItemQueueEvent.TrackingSubjectId.Value == Guid.Empty)
    {
      throw new InvalidOperationException("TrackingSubjectId must be provided for WorkItemQueueEvent");
    }

    await Orchestrator.QueueForStageAsync(
      new QueueForStageCommand(
        workItemQueueEvent.TrackingSubjectId,
        workItemQueueEvent.ProcessingToken,
        workItemQueueEvent.StageId,
        new SimulationCommandContext(
          context.SimulationRunId,
          context.State,
          context.StageStore,
          context.WorkItemStore,
          context.RoutingPolicy)
        ), cancellationToken);


    if (!context.StageStore.StageRuntimeStore.IsBusy(workItemQueueEvent.StageId))
    {
      await Orchestrator.StartProcessingAsync(
        new StartProcessingCommand(
          workItemQueueEvent.StageId,
          new SimulationCommandContext(
              context.SimulationRunId,
              context.State,
              context.StageStore,
              context.WorkItemStore,
              context.RoutingPolicy)
          ),
        cancellationToken);
    }
  }
}
