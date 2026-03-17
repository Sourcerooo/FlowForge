using FlowForge.Domain.Scenarios.ValueObjects;

namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record ScenarioSnapshot(
  ScenarioId ScenarioId,
  string Name,
  TimeSpan PlannedDuration,
  ArrivalProfileSnapshot ArrivalProfile,
  IReadOnlyList<StageConfigurationSnapshot> Stages
  );
