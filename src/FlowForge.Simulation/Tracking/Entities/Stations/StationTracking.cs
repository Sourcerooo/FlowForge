using FlowForge.Domain.Process.ValueObjects;

namespace FlowForge.Simulation.Tracking.Entities.Stations;

public sealed record StationTracking
{
  public StationTracking(StationId stationId, StageId stageId)
  {
    StationId = stationId;
    StageId = stageId;
  }

  public StationId StationId { get; init; }
  public StageId StageId { get; init; }
  public long WorkItemsStartedCount { get; private set; }
  public long WorkItemsCompletedCount { get; private set; }
  public long WorkItemsPlacedOnHoldCount { get; private set; }
  public long WorkItemsCancelledCount { get; private set; }
  public TimeSpan CumulativeProcessingTime { get; private set; }
  public TimeSpan CumulativeOnHoldTime { get; private set; }
  public int PeakBusyWorkers { get; private set; }

  private int _currentActiveWorkers = 0;

  public void StartWorkItem()
  {
    WorkItemsStartedCount += 1;
    IncreaseActiveWorkers();
  }

  public void PutOnHoldWorkItem(TimeSpan processingTime)
  {
    CumulativeProcessingTime += processingTime;
    DecreaseActiveWorkers();
  }

  public void RequeueWorkItem(TimeSpan onHoldTime)
  {
    CumulativeOnHoldTime += onHoldTime;
  }

  public void ResumeWorkItem(TimeSpan onHoldTime)
  {
    CumulativeOnHoldTime += onHoldTime;
    IncreaseActiveWorkers();
  }

  public void CompleteWorkItem(TimeSpan processingTime)
  {
    CumulativeProcessingTime += processingTime;
    WorkItemsCompletedCount++;
    DecreaseActiveWorkers();
  }

  public void CancelProcessingWorkItem(TimeSpan processingTime)
  {
    CumulativeProcessingTime += processingTime;
    DecreaseActiveWorkers();
  }

  public void CancelOnHoldWorkItem(TimeSpan onHoldTime)
  {
    CumulativeOnHoldTime += onHoldTime;
  }

  private void IncreaseActiveWorkers()
  {
    _currentActiveWorkers++;
    if (_currentActiveWorkers > PeakBusyWorkers)
    {
      PeakBusyWorkers = _currentActiveWorkers;
    }
  }

  private void DecreaseActiveWorkers()
  {
    _currentActiveWorkers--;
  }

}
