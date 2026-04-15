using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Simulation.Tracking.Contracts;

namespace FlowForge.Simulation.Tracking.Entities.TrackingSubject;

public class TrackingSubjectStore : ITrackingSubjectStore
{
  private readonly Dictionary<TrackingSubjectId, TrackingSubjectReference> _trackedItem
    = new Dictionary<TrackingSubjectId, TrackingSubjectReference>();
}
