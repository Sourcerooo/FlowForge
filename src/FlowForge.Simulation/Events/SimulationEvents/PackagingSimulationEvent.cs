using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Events.SimulationEvents;

public abstract record PackagingSimulationEvent(
  SimulationEventId Id,
  SimulationRunId SimulationRunId,
  TimeSpan ScheduledTime,
  EventSortRank Rank,
  long SequenceNumber,
  EventKind EventKind
) : SimulationEvent(
    Id,
    SimulationRunId,
    ScheduledTime,
    Rank,
    SequenceNumber,
    EventKind);
