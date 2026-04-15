using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Tracking.Entities.Stations;

namespace FlowForge.Simulation.Tracking.Entities.Stages;

public sealed record StageTracking
{
  public StageId StageId { get; init; }
  public long WorkItemsQueuedCount { get; private set; }
  public long WorkItemsStartedCount { get; private set; }
  public long WorkItemsCompletedCount { get; private set; }
  public long WorkItemsPlacedOnHoldCount { get; private set; }
  public long WorkItemsRequeuedCount { get; private set; }
  public TimeSpan CumulativeQueueWait { get; private set; }
  public TimeSpan CumulativeProcessingTime { get; private set; }
  public TimeSpan CumulativeOnHoldTime { get; private set; }
  public TimeSpan CumulativeBusyTime { get; private set; }
  public int PeakQueueLength { get; private set; }
  public int PeakBusyWorkers { get; private set; }
  public IReadOnlyDictionary<StationId, StationTracking> Stations { get; }
    = new Dictionary<StationId, StationTracking>();
}
