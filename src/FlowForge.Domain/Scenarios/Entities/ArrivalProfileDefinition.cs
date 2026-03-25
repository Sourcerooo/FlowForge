namespace FlowForge.Domain.Scenarios.Entities;

public sealed record ArrivalProfileDefinition(
  TimeSpan GenerationWindow,
  int AverageWorkItemsPerWindow,
  int? MaxWorkItemsPerwindow
  );
