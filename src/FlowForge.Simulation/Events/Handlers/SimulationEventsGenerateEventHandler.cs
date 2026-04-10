using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Scheduling.Contracts;

namespace FlowForge.Simulation.Events.Handlers;


internal sealed class SimulationEventsGenerateEventHandler(ISimulationEventScheduler Scheduler) : ISimulationEventHandler
{
  public EventKind CanHandle() => EventKind.SimulationEventsGenerate;

  private static OrderId GenerateOrder()
  {
    return OrderId.NewId();
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
        var newTrackingEvent = new WorkItemQueueEvent(
          SimulationEventId.NewId(),
          context.SimulationRunId,
          arrivalTime,
          context.State.GetNextSequenceNumber(), null, null, 0, GenerateOrder()
          );

        Scheduler.Schedule(newTrackingEvent);
      }
    }
    var newEvent = new SimulationEventsGenerateEvent(SimulationEventId.NewId(), context.SimulationRunId, nextGenTime, context.State.GetNextSequenceNumber());
    Scheduler.Schedule(newEvent);
  }
}
