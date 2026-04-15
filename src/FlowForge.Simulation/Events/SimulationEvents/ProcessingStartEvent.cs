using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Events.SimulationEvents;

public record ProcessingStartEvent(
  SimulationEventId Id,
  SimulationRunId SimulationRunId,
  TimeSpan ScheduledTime,
  long SequenceNumber,
  StageId StageId)
  : PackagingSimulationEvent(Id, SimulationRunId, ScheduledTime, EventSortRank.ProcessingStart, SequenceNumber,
    EventKind.ProcessingStart);
