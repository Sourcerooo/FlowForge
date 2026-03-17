namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record ThroughputKpiSnapshot(
    int CompletedOrders,
    double OrdersPerSimulatedHour,
    double OrdersPerSimulatedDay);
