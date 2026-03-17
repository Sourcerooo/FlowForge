using FlowForge.Domain.ProcessModel.ValueObjects;

namespace FlowForge.Simulation.Tracking.Entities.Stations;

public sealed class StationTracking
{
  public StationId StationId { get; init; }
  public StageId StageId { get; init; }
  public long WorkItemsStartedCount { get; private set; }
  public long WorkItemsCompletedCount { get; private set; }
  public long WorkItemsPlacedOnHoldCount { get; private set; }
  public long WorkItemsRequeuedCount { get; private set; }
  public TimeSpan CumulativeProcessingTime { get; private set; }
  public TimeSpan CumulativeOnHoldTime { get; private set; }
  public TimeSpan CumulativeBusyTime { get; private set; }
  public int PeakBusyWorkers { get; private set; }
}
