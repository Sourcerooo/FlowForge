using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Application.Services;
using FlowForge.Simulation.Events;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.Services;
using FlowForge.Simulation.Scheduling.Contracts;
using FlowForge.Simulation.Scheduling.Entities;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlowForge.Simulation;

internal static class RootServiceForwarder
{
  internal static void ForwardRootServices(
      IServiceCollection childServices,
      IServiceProvider rootProvider)
  {
    // Logging: Factory forwarden, open generic neu registrieren
    childServices.AddSingleton(rootProvider.GetRequiredService<ILoggerFactory>());
    childServices.Add(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

    // Weitere Cross-Cutting Concerns nach Bedarf:
    // childServices.AddSingleton(rootProvider.GetRequiredService<IConfiguration>());
    // childServices.AddSingleton(rootProvider.GetRequiredService<TimeProvider>());
  }
}

public static class DependencyInjection
{
  public static IServiceCollection AddSimulation(this IServiceCollection services)
  {
    services.AddScoped<ISimulationScopeFactory, SimulationScopeFactory>();
    services.AddScoped<ISimulationRunner, SimulationRunner>();
    return services;
  }

  public static IServiceCollection AddScopedSimulation(IServiceCollection services)
  {
    services.AddSingleton<ISimulationContextBuilder, SimulationContextBuilder>();

    services.AddSingleton<ISimulationContextBuilder, SimulationContextBuilder>();
    services.AddSingleton<IWorkItemTrackingStore, WorkItemTrackingStore>();
    services.AddSingleton<IStageTrackingStore, StageTrackingStore>();
    services.AddSingleton<IWorkItemRuntimeStateStore, WorkItemRuntimeStateStore>();
    services.AddSingleton<IStageRuntimeStateStore, StageRuntimeStateStore>();

    services.AddSingleton<IStageService, StageService>();
    services.AddSingleton<IWorkItemService, WorkItemService>();

    services.AddSingleton<ISimulationEventQueue, SimulationEventPriorityQueue>();
    EventsDependencyInjection.AddEvents(services);
    services.AddSingleton<ISimulationEventScheduler, SimulationEventScheduler>();

    services.AddSingleton<IRoutingPolicy, RoutingPolicy>();

    services.AddSingleton<IWorkItemProcessOrchestrator, WorkItemProcessOrchestrator>();

    services.AddSingleton<ISimulationEngine, SimulationEngine>();
    return services;
  }
}
