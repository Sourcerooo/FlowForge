using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Tracking.Entities.TrackingSubject;

public class TrackingSubjectStore : ITrackingSubjectStore
{
  private readonly Dictionary<TrackingSubjectId, TrackingSubjectReference> _trackedItem
    = new Dictionary<TrackingSubjectId, TrackingSubjectReference>();
}
