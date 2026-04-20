using FlowForge.Domain.Orders.ValueObjects;

namespace FlowForge.Simulation.Runtime.ValueObjects;

public readonly record struct StationProcessingInfo(
  TrackingSubjectId TrackingSubjectId,
  int WorkerSlot,
  TimeSpan StartedAt
  );
