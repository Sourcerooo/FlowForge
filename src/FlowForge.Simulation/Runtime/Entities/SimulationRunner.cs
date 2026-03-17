using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Enums;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationRunner : ISimulationRunner
{
  public async Task<SimulationRunResult> RunSimulation(SimulationExecutionContext context, CancellationToken cancellationToken)
  {

    while (context.Data.EventQueue.TryDequeue(out var nextEvent))
    {
      if (cancellationToken.IsCancellationRequested)
      {
        return SimulationRunResult.Cancelled;
      }

      context.Data.State.AdvanceTo(nextEvent.ScheduledTime);
      await context.Service.Dispatcher.DispatchAsync(nextEvent,
        context.CreateHandlerContext(),
        cancellationToken);

    }

    return SimulationRunResult.Completed;
  }
}
