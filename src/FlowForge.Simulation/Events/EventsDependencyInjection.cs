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
    services.AddSingleton<ISimulationEventHandler, ProcessingCompleteEventHandler>();
    services.AddSingleton<ISimulationEventHandler, ProcessingStartEventHandler>();
    services.AddSingleton<ISimulationEventHandler, WorkItemCompleteEventHandler>();
    return services;
  }
}
