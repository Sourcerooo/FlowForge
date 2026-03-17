using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Tracking.Entities.TrackingSubject;

public sealed record TrackingSubjectReference(
  TrackingSubjectId TrackingSubjectId,
  string EntityType,
  Guid ExternalEntityId
  );
