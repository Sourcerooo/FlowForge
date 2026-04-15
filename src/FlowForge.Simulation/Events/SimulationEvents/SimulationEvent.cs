using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Events.SimulationEvents;

public abstract record SimulationEvent(
  SimulationEventId Id,
  SimulationRunId SimulationRunId,
  TimeSpan ScheduledTime,
  EventSortRank Rank,
  long SequenceNumber,
  EventKind EventKind);
