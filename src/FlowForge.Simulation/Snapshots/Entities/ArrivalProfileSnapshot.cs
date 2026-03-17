namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record ArrivalProfileSnapshot(
  TimeSpan GenerationWindow,
  int AverageOrdersPerWindow,
  int? MaxOrdersPerWindow
  );
