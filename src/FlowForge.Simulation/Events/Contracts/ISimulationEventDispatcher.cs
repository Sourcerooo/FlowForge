using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Runtime.Entities;

namespace FlowForge.Simulation.Events.Contracts;

public interface ISimulationEventDispatcher
{
  public Task DispatchAsync(
    SimulationEvent simulationEvent,
    SimulationExecutionHandlerContext context,
    CancellationToken cancellationToken);

}
