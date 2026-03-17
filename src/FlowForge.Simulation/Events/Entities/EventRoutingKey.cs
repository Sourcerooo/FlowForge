using FlowForge.Domain.ProcessModel.ValueObjects;
using FlowForge.Simulation.Events.Enums;

namespace FlowForge.Simulation.Events.Entities;

public readonly record struct EventRoutingKey(
  EventKind EventKind,
  StageId? StageId
  );
