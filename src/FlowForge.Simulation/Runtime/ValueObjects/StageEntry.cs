using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;

namespace FlowForge.Simulation.Runtime.ValueObjects;

public readonly record struct StageEntry(
  TrackingSubjectId TrackingSubjectId,
  TimeSpan EnqueuedAt = default,
  TimeSpan StartedAt = default,
  TimeSpan CompletedAt = default,
  TimeSpan StoppedAt = default,
  StageId? StageId = default,
  StationId? StationId = default
  );
