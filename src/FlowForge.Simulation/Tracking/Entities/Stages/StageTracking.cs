using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Tracking.Entities.Stations;

namespace FlowForge.Simulation.Tracking.Entities.Stages;


public enum OnHoldOccurrence
{
  First = 1,
  Repeated = 2
}
public enum OnQueueOccurrence
{
  First = 1,
  Requeued = 2
}


public sealed record StageTracking
{
  public StageId StageId { get; init; }
  public long WorkItemsQueuedCount { get; private set; }
  public long WorkItemsStartedCount { get; private set; }
  public long WorkItemsCompletedCount { get; private set; }
  public long TransactionsOnHoldCount { get; private set; }
  public long WorkItemsUniqueOnHoldCount { get; private set; }
  public long WorkItemsRequeuedCount { get; private set; }
  public TimeSpan CumulativeQueueWait { get; private set; }
  public TimeSpan CumulativeProcessingTime { get; private set; }
  public TimeSpan CumulativeOnHoldTime { get; private set; }
  public int PeakQueueLength { get; private set; }
  public int PeakBusyWorkers { get; private set; }
  public IReadOnlyDictionary<StationId, StationTracking> Stations { get; }
    = new Dictionary<StationId, StationTracking>();

  public int CurrentQueueLength { get; private set; }
  public int CurrentBusyWorkers { get; private set; }

  public void EnqueueWorkItem(
    OnQueueOccurrence onQueueOccurrence,
    TimeSpan processingTime = default
    )
  {
    EnqueueItem(onQueueOccurrence);
    if (onQueueOccurrence == OnQueueOccurrence.Requeued)
    {
      CumulativeProcessingTime += processingTime;
      CurrentBusyWorkers--;
    }
  }
  public enum ProcessingKind
  {
    InitialStartFromQueue,
    ResumeFromOnHold,
    ResumeFromQueue
  }

  public void StartProcessingWorkItem(ProcessingKind entryKind,
    TimeSpan queueWaitTime = default,
    TimeSpan onHoldTime = default)
  {
    switch (entryKind)
    {
      case ProcessingKind.InitialStartFromQueue:
        WorkItemsStartedCount++;
        CumulativeQueueWait += queueWaitTime;
        CurrentQueueLength--;
        break;
      case ProcessingKind.ResumeFromQueue:
        CumulativeQueueWait += queueWaitTime;
        CurrentQueueLength--;
        break;
      case ProcessingKind.ResumeFromOnHold:
        CumulativeOnHoldTime += onHoldTime;
        break;
    }
    CurrentBusyWorkers++;
    if (CurrentBusyWorkers > PeakBusyWorkers)
    {
      PeakBusyWorkers = CurrentBusyWorkers;
    }
  }

  public void CompleteWorkItem(TimeSpan processingTime)
  {
    CumulativeProcessingTime += processingTime;
    WorkItemsCompletedCount++;
    CurrentBusyWorkers--;
  }


  public void StopWorkItem(TimeSpan processingTime, OnHoldOccurrence onHoldOccurrence)
  {
    StopItem(onHoldOccurrence);
    CumulativeProcessingTime += processingTime;

  }

  public void StopAndRequeueWorkItem(TimeSpan processingTime, OnHoldOccurrence onHoldOccurrence)
  {
    StopItem(onHoldOccurrence);
    EnqueueItem(OnQueueOccurrence.Requeued);
    CumulativeProcessingTime += processingTime;
  }

  private void StopItem(OnHoldOccurrence onHoldOccurrence)
  {
    if (onHoldOccurrence == OnHoldOccurrence.First)
    {
      WorkItemsUniqueOnHoldCount++;
    }
    TransactionsOnHoldCount++;
    CurrentBusyWorkers--;
  }

  private void EnqueueItem(OnQueueOccurrence onQueueOccurrence)
  {
    if (onQueueOccurrence == OnQueueOccurrence.First)
    {
      WorkItemsQueuedCount++;
    }
    else if (onQueueOccurrence == OnQueueOccurrence.Requeued)
    {
      WorkItemsRequeuedCount++;
    }
    CurrentQueueLength++;
    if (CurrentQueueLength > PeakQueueLength)
    {
      PeakQueueLength = CurrentQueueLength;
    }
  }
}
