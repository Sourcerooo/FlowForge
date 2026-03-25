using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;

namespace FlowForge.Simulation.Scheduling;

internal readonly record struct SimulationEventPriorityQueueKey(TimeSpan ScheduledTime,
  EventSortRank Rank,
  long SequenceNumber) : IComparable<SimulationEventPriorityQueueKey>
{
  internal SimulationEventPriorityQueueKey(SimulationEvent simulationEvent)
    : this(simulationEvent.ScheduledTime, simulationEvent.Rank, simulationEvent.SequenceNumber) { }

  public int CompareTo(SimulationEventPriorityQueueKey other)
  {
    return (ScheduledTime, Rank, SequenceNumber)
        .CompareTo((other.ScheduledTime, other.Rank, other.SequenceNumber));
  }
}
