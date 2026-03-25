using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Runtime.Entities;

namespace FlowForge.Simulation.Events.Contracts;

public interface ISimulationEventHandler
{
  public EventKind CanHandle();
  public Task Process(SimulationEvent simulationEvent, SimulationExecutionHandlerContext context, CancellationToken cancellationToken);
}
