namespace FlowForge.Simulation.Events.Enums;

public enum EventKind
{
  SimulationEventsGenerate = 0,
  OrderQueue = 1,
  OrderComplete = 2,
  ProcessingStart = 3,
  ProcessingComplete = 4,
  SnapshotCreate = 5,
  DisruptionRaise = 100,
  DisruptionClear = 101
}
