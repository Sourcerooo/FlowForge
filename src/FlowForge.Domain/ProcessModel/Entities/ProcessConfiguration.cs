namespace FlowForge.Simulation.Runtime.Entities;

public sealed record ProcessConfiguration(
  ProcessConfigurationId ProcessConfigurationId,
  string ProcessKey,
  string Name,
  DateTime StartTime,
  TimeSpan PlannedDuration,
  ArrivalProfileDefinition ArrivalProfileDefinition,
  IReadOnlyList<StageDefinition> Stages
  );
