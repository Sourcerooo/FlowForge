using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Scheduling.Contracts;

namespace FlowForge.Simulation.Scheduling.Entities;

internal sealed class SimulationEventPriorityQueue : ISimulationEventQueue
{
  private readonly PriorityQueue<SimulationEvent, SimulationEventPriorityQueueKey> _priorityQueue = new PriorityQueue<SimulationEvent, SimulationEventPriorityQueueKey>();
  public SimulationEvent? Peek()
  {
    return _priorityQueue.Peek();
  }
  public void Queue(SimulationEvent nextEvent)
  {
    _priorityQueue.Enqueue(nextEvent, new SimulationEventPriorityQueueKey(nextEvent));
  }

  public bool TryDequeue(out SimulationEvent? nextEvent)
  {
    return _priorityQueue.TryDequeue(out nextEvent, out _);
  }
}
