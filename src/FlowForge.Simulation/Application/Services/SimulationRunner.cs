using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Application.Entities;
using FlowForge.Simulation.Runtime.Contracts;
using Microsoft.Extensions.Logging;

namespace FlowForge.Simulation.Application.Services;

public sealed class SimulationRunner(
    ISimulationScopeFactory scopeFactory,
    ILogger<SimulationRunner> logger) : ISimulationRunner
{
  private readonly ISimulationScopeFactory _scopeFactory = scopeFactory;
  private readonly ILogger<SimulationRunner> _logger = logger;

  public async Task RunAsync(CancellationToken ct = default)
  {
    var parameters = SimulationParametersFactory.Create();

    await using var scope = _scopeFactory.CreateScope(parameters);

    _logger.LogInformation("Starting simulation: {Name}", parameters.Name);

    var simulationContextBuilder = scope.GetService<ISimulationContextBuilder>();
    var simulationContext = simulationContextBuilder.Build(parameters);

    var result = await scope.Engine.RunSimulationAsync(simulationContext, ct);

    _logger.LogInformation("Simulation stopped in status {Status}", result);
  }
}
