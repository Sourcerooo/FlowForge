using FlowForge.Domain.ProcessModel.ValueObjects;

namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record StageConfigurationSnapshot(
  StageId StageId,
  string DisplayName,
  int WorkerCount,
  TimeSpan AverageProcessingTime
  );
