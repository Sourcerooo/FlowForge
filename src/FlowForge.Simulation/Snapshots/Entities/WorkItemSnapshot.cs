using FlowForge.Domain.ProcessModel.ValueObjects;
using FlowForge.Simulation.Tracking.Enums;
using FlowForge.Simulation.Tracking.ValueObjects;

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
