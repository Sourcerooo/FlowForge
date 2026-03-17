namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record LeadTimeKpiSnapshot(
    TimeSpan Average,
    TimeSpan Min,
    TimeSpan Max);
