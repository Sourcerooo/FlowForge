namespace FlowForge.Simulation.Application.Contracts;

public interface ISimulationRunner
{
  public Task RunAsync(CancellationToken ct = default);
}
