using FlowForge.Application.Simulation;
using FlowForge.Application.Simulation.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<ISimulationContextBuilder, SimulationContextBuilder>();
    return services;
  }
}
