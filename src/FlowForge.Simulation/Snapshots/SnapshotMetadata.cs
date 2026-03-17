namespace FlowForge.Simulation.Snapshots;

public sealed record SnapshotMetadata(
    DateTimeOffset PublishedAtUtc,
    string PublishReason,
    int SchemaVersion);
