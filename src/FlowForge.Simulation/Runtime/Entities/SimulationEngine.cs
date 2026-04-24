using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Scheduling.Contracts;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationEngine(
  ISimulationEventQueue EventQueue,
  ISimulationEventScheduler Scheduler,
  ISimulationEventDispatcher Dispatcher)
  : ISimulationEngine
{
  public async Task<SimulationRunResult> RunSimulationAsync(SimulationExecutionContext context, CancellationToken cancellationToken)
  {
    Scheduler.Schedule(
        new SimulationEventsGenerateEvent(
          SimulationEventId.NewId(),
          context.SimulationRunId,
          TimeSpan.FromSeconds(0),
          context.State.GetNextSequenceNumber())
      );

    while (EventQueue.TryDequeue(out var nextEvent)
      && context.State.CurrentTime < context.ProcessConfiguration.PlannedDuration
      && nextEvent is not null
      && nextEvent.ScheduledTime < context.ProcessConfiguration.PlannedDuration
      )
    {
      if (cancellationToken.IsCancellationRequested)
      {
        return SimulationRunResult.Cancelled;
      }

      context.State.AdvanceTo(nextEvent.ScheduledTime);
      await Dispatcher.DispatchAsync(nextEvent,
        context.CreateHandlerContext(),
        cancellationToken);

    }
    return SimulationRunResult.Completed;
  }
}
