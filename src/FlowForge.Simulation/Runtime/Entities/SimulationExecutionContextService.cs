using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Kpi.Contracts;
using FlowForge.Simulation.Scheduling.Contracts;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class SimulationExecutionContextService
{
  public ISimulationEventScheduler Scheduler { get; init; } = default!;
  public ISimulationEventDispatcher Dispatcher { get; init; } = default!;
  public IEventHandlerRegistry HandlerRegistry { get; init; } = default!;
  public IKpiCollector KpiCollector { get; init; } = default!;
  public ISnapshotBuilder SnapshotBuilder { get; init; } = default!;
}
