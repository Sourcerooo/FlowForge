using FlowForge.Domain.Process.ValueObjects;

namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record StageConfigurationSnapshot(
  StageId StageId,
  string DisplayName,
  int WorkerCount,
  TimeSpan AverageProcessingTime
  );
