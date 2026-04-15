using FlowForge.Domain.Process.Entities;
using FlowForge.Simulation.Runtime.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Simulation.Application.Entities;

public sealed class SimulationScope : IAsyncDisposable
{
  private readonly ServiceProvider _serviceProvider;

  private SimulationScope(ServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public ISimulationEngine Engine =>
      _serviceProvider.GetRequiredService<ISimulationEngine>();

  public T GetService<T>() where T : notnull =>
    _serviceProvider.GetRequiredService<T>();

  public async ValueTask DisposeAsync()
  {
    await _serviceProvider.DisposeAsync();
  }

  internal static SimulationScope Create(
      ProcessConfiguration processConfiguration,
      IServiceProvider rootProvider)
  {
    var services = new ServiceCollection();
    services.AddSingleton(processConfiguration);
    RootServiceForwarder.ForwardRootServices(services, rootProvider);
    DependencyInjection.AddScopedSimulation(services);

    var provider = services.BuildServiceProvider(new ServiceProviderOptions
    {
      ValidateOnBuild = true,
      ValidateScopes = true
    });
    var simulationScope = new SimulationScope(provider);
    return simulationScope;
  }
}
