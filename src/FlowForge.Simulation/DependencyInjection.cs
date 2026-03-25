using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Entities;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Scheduling.Contracts;
using FlowForge.Simulation.Scheduling.Entities;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Simulation;

public static class DependencyInjection
{
  public static IServiceCollection AddSimulation(this IServiceCollection services)
  {
    services.AddSingleton<IWorkItemTrackingStore, WorkItemTrackingStore>();
    services.AddSingleton<ISimulationEventQueue, SimulationEventPriorityQueue>();
    services.AddSingleton<ISimulationEventScheduler, SimulationEventScheduler>();
    services.AddSingleton<ISimulationRunner, SimulationRunner>();
    services.AddSingleton<ISimulationEventDispatcher, SimulationEventDispatcher>();
    return services;
  }
}
