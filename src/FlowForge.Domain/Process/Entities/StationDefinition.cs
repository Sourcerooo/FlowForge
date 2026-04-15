using FlowForge.Domain.Process.ValueObjects;

namespace FlowForge.Domain.Process.Entities;

public sealed record StationDefinition(
  StationId StationId,
  StageId StageId,
  string StationKey,
  string Name,
  int WorkerCount,
  TimeSpan AverageProcessingTime
  );
