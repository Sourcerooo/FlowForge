using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Application.Services;
using FlowForge.Simulation.Events;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.Services;
using FlowForge.Simulation.Scheduling.Contracts;
using FlowForge.Simulation.Scheduling.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Simulation;

public static class DependencyInjection
{
  public static IServiceCollection AddSimulation(this IServiceCollection services)
  {
    services.AddScoped<ISimulationEventQueue, SimulationEventPriorityQueue>();
    services.AddScoped<ISimulationEventScheduler, SimulationEventScheduler>();
    services.AddScoped<ISimulationContextBuilder, SimulationContextBuilder>();
    services.AddScoped<IWorkItemProcessOrchestrator, WorkItemProcessOrchestrator>();
    EventsDependencyInjection.AddEvents(services);
    services.AddScoped<IStageService, StageService>();
    services.AddScoped<IWorkItemService, WorkItemService>();
    services.AddScoped<ISimulationRunner, SimulationRunner>();

    return services;
  }
}
