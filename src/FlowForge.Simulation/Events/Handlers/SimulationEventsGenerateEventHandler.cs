using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Application.ValueObjects;
using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Scheduling.Contracts;

namespace FlowForge.Simulation.Events.Handlers;


internal sealed class SimulationEventsGenerateEventHandler(ISimulationEventScheduler Scheduler, IWorkItemProcessOrchestrator Orchestrator) : ISimulationEventHandler
{
  public EventKind CanHandle() => EventKind.SimulationEventsGenerate;

  private static TrackingSubjectId GenerateOrder()
  {
    return TrackingSubjectId.NewId();
  }
  public async Task Process(
    SimulationEvent simulationEvent,
    SimulationExecutionHandlerContext context,
    CancellationToken cancellationToken)
  {
    var simulationEventsGenerateEvent = (SimulationEventsGenerateEvent)simulationEvent;
    var curTime = context.State.CurrentTime;
    var nextGenTime = simulationEventsGenerateEvent.ScheduledTime + context.ProcessConfiguration.ArrivalProfileDefinition.GenerationWindow;
    for (var i = 0; i < context.ProcessConfiguration.ArrivalProfileDefinition.AverageWorkItemsPerWindow; i++)
    {
      var arrivalTime = curTime + TimeSpan.FromMinutes(Random.Shared.Next(0, (int)context.ProcessConfiguration.ArrivalProfileDefinition.GenerationWindow.TotalMinutes));
      if (arrivalTime < nextGenTime)
      {
        await Orchestrator.CreateFromGenerationAsync(
          new CreateFromGenerationCommand(
            GenerateOrder(),
            arrivalTime,
            new SimulationCommandContext(
              context.SimulationRunId,
              context.State,
              context.StageStore,
              context.WorkItemStore,
              context.RoutingPolicy)
            ), cancellationToken);
      }
    }
    var newEvent = new SimulationEventsGenerateEvent(SimulationEventId.NewId(), context.SimulationRunId, nextGenTime, context.State.GetNextSequenceNumber());
    Scheduler.Schedule(newEvent);
  }
}
