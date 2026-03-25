using FlowForge.Domain.ProcessModel.ValueObjects;
using FlowForge.Domain.Scenarios.Entities;

namespace FlowForge.Domain.ProcessModel.Entities;

public sealed record ProcessConfiguration(
  ProcessConfigurationId ProcessConfigurationId,
  string ProcessKey,
  string Name,
  DateTime StartTime,
  TimeSpan PlannedDuration,
  ArrivalProfileDefinition ArrivalProfileDefinition,
  IReadOnlyList<StageDefinition> Stages
  );
