using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Entities;
using FlowForge.Simulation.Events.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Simulation.Events;

public static class EventsDependencyInjection
{
  public static IServiceCollection AddEvents(this IServiceCollection services)
  {
    services.AddScoped<ISimulationEventDispatcher, SimulationEventDispatcher>();
    //Register event handlers
    services.AddScoped<ISimulationEventHandler, SimulationEventsGenerateEventHandler>();
    services.AddScoped<ISimulationEventHandler, WorkItemQueueEventHandler>();
    return services;
  }
}
