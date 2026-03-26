using FlowForge.Domain.Process.ValueObjects;

namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record BottleneckKpiSnapshot(
    StageId? StageId,
    string? StageName,
    double Score,
    string Reason);
