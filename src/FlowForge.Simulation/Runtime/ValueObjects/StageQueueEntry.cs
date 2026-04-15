using FlowForge.Domain.Orders.ValueObjects;

namespace FlowForge.Simulation.Runtime.ValueObjects;

public readonly record struct StageQueueEntry(
  TrackingSubjectId TrackingSubjectId,
  TimeSpan EnqueuedAt,
  ProcessingToken ProcessingToken = default
  );
