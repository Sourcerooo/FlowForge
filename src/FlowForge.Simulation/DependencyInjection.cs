using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Entities;
using FlowForge.Simulation.Events.Handlers;
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
    services.AddScoped<IWorkItemTrackingStore, WorkItemTrackingStore>();
    services.AddScoped<ISimulationEventQueue, SimulationEventPriorityQueue>();
    services.AddScoped<ISimulationEventScheduler, SimulationEventScheduler>();
    services.AddScoped<ISimulationRunner, SimulationRunner>();
    services.AddScoped<ISimulationEventDispatcher, SimulationEventDispatcher>();
    services.AddScoped<ISimulationEventHandler, SimulationEventsGenerateEventHandler>();
    services.AddScoped<ISimulationEventHandler, WorkItemQueueEventHandler>();
    return services;
  }
}
