namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record KpiTrendPointSnapshot(
    TimeSpan SimulationTime,
    int WorkInProgress,
    double ThroughputPerHour);
