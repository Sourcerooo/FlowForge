using FlowForge.Simulation.Checkpoints.Documents;

namespace FlowForge.Application.Checkpoints.Contracts;

public interface ISimulationCheckpointStore
{
    public Task SaveAsync(
        SimulationStateDocument state,
        string filePath,
        CancellationToken cancellationToken = default);

    public Task<SimulationStateDocument> LoadAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}
