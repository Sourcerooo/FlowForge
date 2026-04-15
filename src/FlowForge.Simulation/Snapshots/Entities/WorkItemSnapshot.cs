using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Tracking.Enums;

namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record WorkItemSnapshot(
  TrackingSubjectId TrackingSubjectId,
  WorkItemStatus Status,
  TimeSpan CreatedAt,
  StageId? StageId,
  StationId? StationId,
  TimeSpan? QueueEnteredAt,
  TimeSpan? ProcessingStartedAt,
  TimeSpan TimeInSystem,
  double Progress
  );
