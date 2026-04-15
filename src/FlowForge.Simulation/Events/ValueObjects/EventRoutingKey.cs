using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Events.Enums;

namespace FlowForge.Simulation.Events.ValueObjects;

public readonly record struct EventRoutingKey(EventKind EventKind, StageId? StageId);
