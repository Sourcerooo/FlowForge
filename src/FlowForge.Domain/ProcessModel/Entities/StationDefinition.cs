using FlowForge.Domain.ProcessModel.ValueObjects;

namespace FlowForge.Domain.ProcessModel.Entities;

public sealed record StationDefinition(
  StationId StationId,
  StageId StageId,
  string StationKey,
  string Name,
  int WorkerCount,
  TimeSpan AverageProcessingTime
  );
