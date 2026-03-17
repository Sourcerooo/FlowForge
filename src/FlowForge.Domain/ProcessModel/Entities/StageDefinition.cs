using FlowForge.Domain.ProcessModel.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed record StageDefinition(
  StageId StageId,
  string StageKey,
  string Name,
  int Sequence,
  IReadOnlyList<StationDefinition> Stations
  );
