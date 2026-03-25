using FlowForge.Domain.ProcessModel.ValueObjects;

namespace FlowForge.Domain.ProcessModel.Entities;

public sealed record StageDefinition(
  StageId StageId,
  string StageKey,
  string Name,
  int Sequence,
  IReadOnlyList<StationDefinition> Stations
  );
