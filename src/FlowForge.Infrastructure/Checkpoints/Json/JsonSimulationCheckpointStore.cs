using System.Text.Json;
using FlowForge.Application.Checkpoints.Contracts;
using FlowForge.Simulation.Checkpoints.Documents;

namespace FlowForge.Infrastructure.Checkpoints.Json;

public sealed class JsonSimulationCheckpointStore : ISimulationCheckpointStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public async Task SaveAsync(
        SimulationStateDocument state,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(state);

        var document = new SimulationCheckpointDocument(
            FormatVersion: 1,
            RunMetadata: state.RunMetadata with { LastSavedAtUtc = DateTimeOffset.UtcNow },
            ProcessConfiguration: state.ProcessConfiguration,
            RunOptions: state.RunOptions,
            RuntimeState: state.RuntimeState,
            EventQueue: state.EventQueue,
            Tracking: state.Tracking,
            KpiState: state.KpiState,
            SnapshotState: state.SnapshotState);

        var directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, document, SerializerOptions, cancellationToken);
    }

    public async Task<SimulationStateDocument> LoadAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Checkpoint file was not found.", filePath);
        }

        await using var stream = File.OpenRead(filePath);
        var document = await JsonSerializer.DeserializeAsync<SimulationCheckpointDocument>(
            stream,
            SerializerOptions,
            cancellationToken)
            ?? throw new InvalidDataException("Checkpoint file could not be deserialized.");

        return new SimulationStateDocument(
            SimulationRunId: document.RunMetadata.SimulationRunId,
            RunMetadata: document.RunMetadata,
            ProcessConfiguration: document.ProcessConfiguration,
            RunOptions: document.RunOptions,
            RuntimeState: document.RuntimeState,
            EventQueue: document.EventQueue,
            Tracking: document.Tracking,
            KpiState: document.KpiState,
            SnapshotState: document.SnapshotState);
    }
}
