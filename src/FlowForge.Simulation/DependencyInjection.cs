using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Simulation;

public static class DependencyInjection
{
  public static IServiceCollection AddSimulation(this IServiceCollection services)
  {
    services.AddSingleton<IWorkItemTrackingStore, WorkItemTrackingStore>();
    return services;
  }
}
