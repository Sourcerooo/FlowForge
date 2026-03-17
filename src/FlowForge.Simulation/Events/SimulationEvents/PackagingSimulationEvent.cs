using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.ProcessModel.ValueObjects;
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
  EventKind EventKind,
  StageId? StageId,
  StationId? StationId,
  long? ProcessingToken,
  OrderId? OrderId
) : SimulationEvent(
    Id,
    SimulationRunId,
    ScheduledTime,
    Rank,
    SequenceNumber,
    EventKind,
    StageId,
    StationId,
    ProcessingToken);
