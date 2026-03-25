using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Scheduling.Contracts;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationRunner(
  ISimulationEventQueue EventQueue,
  ISimulationEventDispatcher Dispatcher)
  : ISimulationRunner
{
  public async Task<SimulationRunResult> RunSimulation(SimulationExecutionContext context, CancellationToken cancellationToken)
  {

    while (EventQueue.TryDequeue(out var nextEvent))
    {
      if (cancellationToken.IsCancellationRequested)
      {
        return SimulationRunResult.Cancelled;
      }
      if (nextEvent is null)
      {
        continue;
      }

      context.Data.State.AdvanceTo(nextEvent.ScheduledTime);
      await Dispatcher.DispatchAsync(nextEvent,
        context.CreateHandlerContext(),
        cancellationToken);

    }

    return SimulationRunResult.Completed;
  }
}
