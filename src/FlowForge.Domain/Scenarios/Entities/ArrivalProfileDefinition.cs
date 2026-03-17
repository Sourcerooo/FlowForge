namespace FlowForge.Simulation.Runtime.Entities;

public sealed record ArrivalProfileDefinition(
  TimeSpan GenerationWindow,
  int AverageWorkItemsPerWindow,
  int? MaxWorkItemsPerwindow
  );
