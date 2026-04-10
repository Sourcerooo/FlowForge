using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Runtime.ValueObjects;

public readonly record struct StageQueueEntry(
  TrackingSubjectId TrackingSubjectId,
  TimeSpan EnqueuedAt
  );
