namespace FlowForge.Simulation.Events.Enums;

public enum EventSortRank
{
  Highest = 0,
  //Completion = 100,
  ProcessingComplete = 110,
  OrderComplete = 115,
  DisruptionClear = 120,
  DisruptionRaise = 130,
  //Routing = 200,
  OrderQueue = 210,
  //Start = 300,
  ProcessingStart = 310,
  //Generation = 400,
  SimulationEventsGenerate = 410,
  //Snapshot = 500,
  SnapshotGenerate = 510,
  Lowest = 1000
};
