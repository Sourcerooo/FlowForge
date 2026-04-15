using FlowForge.Domain.Process.ValueObjects;

namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record StageKpiSnapshot(
    StageId StageId,
    string StageName,
    int QueueLength,
    TimeSpan AverageQueueWait,
    TimeSpan AverageProcessingTime,
    double Utilization,
    int OrdersProcessed);
