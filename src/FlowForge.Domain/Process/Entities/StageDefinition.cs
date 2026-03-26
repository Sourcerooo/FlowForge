using FlowForge.Domain.Process.ValueObjects;

namespace FlowForge.Domain.Process.Entities;

public sealed record StageDefinition(
  StageId StageId,
  string StageKey,
  string Name,
  int Sequence,
  IReadOnlyList<StationDefinition> Stations
  );
