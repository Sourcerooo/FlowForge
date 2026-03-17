using FlowForge.Domain.ProcessModel.ValueObjects;
using FlowForge.Simulation.Events.Enums;

namespace FlowForge.Simulation.Events.ValueObjects;

internal readonly record struct EventRoutingKey(EventKind EventKind, StageId? StageId);
