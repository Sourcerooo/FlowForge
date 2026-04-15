using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Entities;
using FlowForge.Simulation.Events.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Simulation.Events;

public static class EventsDependencyInjection
{
  public static IServiceCollection AddEvents(this IServiceCollection services)
  {
    services.AddSingleton<ISimulationEventDispatcher, SimulationEventDispatcher>();
    //Register event handlers
    services.AddSingleton<ISimulationEventHandler, SimulationEventsGenerateEventHandler>();
    services.AddSingleton<ISimulationEventHandler, WorkItemQueueEventHandler>();
    return services;
  }
}
