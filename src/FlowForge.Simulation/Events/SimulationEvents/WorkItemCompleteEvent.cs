using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Events.SimulationEvents;

public record WorkItemCompleteEvent(
  SimulationEventId Id,
  SimulationRunId SimulationRunId,
  TimeSpan ScheduledTime,
  long SequenceNumber,
  long ProcessingToken,
  TrackingSubjectId TrackingSubjectId)
  : PackagingSimulationEvent(Id, SimulationRunId, ScheduledTime, EventSortRank.OrderComplete, SequenceNumber,
    EventKind.WorkItemComplete);
