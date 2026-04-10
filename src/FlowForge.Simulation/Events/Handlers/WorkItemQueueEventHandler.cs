using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Scheduling.Contracts;

namespace FlowForge.Simulation.Events.Handlers;

internal sealed class WorkItemQueueEventHandler(ISimulationEventScheduler Scheduler) : ISimulationEventHandler
{
  public EventKind CanHandle() => EventKind.WorkItemQueue;

  private static StageId GetFirstStage(IEnumerable<StageDefinition> stages)
  {
    return stages.FirstOrDefault()?.StageId ?? throw new InvalidOperationException("Process must have at least one stage");
  }

  public async Task Process(
    SimulationEvent simulationEvent,
    SimulationExecutionHandlerContext context,
    CancellationToken cancellationToken)
  {
    var workItemQueueEvent = (WorkItemQueueEvent)simulationEvent;
    var curTime = context.State.CurrentTime;
    var stageId = GetFirstStage(context.ProcessConfiguration.Stages);
    var newTrackingEvent = new ProcessingStartEvent(
          SimulationEventId.NewId(),
          context.SimulationRunId,
          curTime,
          context.State.GetNextSequenceNumber(), stageId, null, 0, workItemQueueEvent.OrderId
        );
    Scheduler.Schedule(newTrackingEvent);
  }
}
