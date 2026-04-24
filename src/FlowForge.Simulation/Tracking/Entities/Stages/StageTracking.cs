using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;
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
    StageEntry stageEntry,
    OnQueueOccurrence onQueueOccurrence)
  {
    EnqueueItem(onQueueOccurrence);
    if (onQueueOccurrence == OnQueueOccurrence.Requeued)
    {
      CumulativeProcessingTime += GetDuration(stageEntry.StartedAt, stageEntry.StoppedAt, nameof(stageEntry.StoppedAt));
    }
  }
  public enum ProcessingKind
  {
    InitialStartFromQueue,
    ResumeFromOnHold,
    ResumeFromQueue
  }

  public void StartProcessingWorkItem(StageEntry stageEntry, ProcessingKind entryKind)
  {
    switch (entryKind)
    {
      case ProcessingKind.InitialStartFromQueue:
        WorkItemsStartedCount++;
        CumulativeQueueWait += GetDuration(stageEntry.EnqueuedAt, stageEntry.StartedAt, nameof(stageEntry.StartedAt));
        CurrentQueueLength--;
        break;
      case ProcessingKind.ResumeFromQueue:
        CumulativeQueueWait += GetDuration(stageEntry.RequeuedAt, stageEntry.StartedAt, nameof(stageEntry.StartedAt));
        CurrentQueueLength--;
        break;
      case ProcessingKind.ResumeFromOnHold:
        CumulativeOnHoldTime += GetDuration(stageEntry.StoppedAt, stageEntry.StartedAt, nameof(stageEntry.StartedAt));
        break;
    }
    CurrentBusyWorkers++;
    if (CurrentBusyWorkers > PeakBusyWorkers)
    {
      PeakBusyWorkers = CurrentBusyWorkers;
    }
  }

  public void CompleteWorkItem(StageEntry stageEntry)
  {
    CumulativeProcessingTime += GetDuration(stageEntry.StartedAt, stageEntry.CompletedAt, nameof(stageEntry.CompletedAt));
    WorkItemsCompletedCount++;
    CurrentBusyWorkers--;
  }


  public void PutOnHoldWorkItem(StageEntry stageEntry, OnHoldOccurrence onHoldOccurrence)
  {
    PutOnHoldItem(onHoldOccurrence);
    CumulativeProcessingTime += GetDuration(stageEntry.StartedAt, stageEntry.StoppedAt, nameof(stageEntry.StoppedAt));
  }

  public void StopAndRequeueWorkItem(StageEntry stageEntry, OnHoldOccurrence onHoldOccurrence)
  {
    PutOnHoldItem(onHoldOccurrence);
    EnqueueItem(OnQueueOccurrence.Requeued);
    CumulativeProcessingTime += GetDuration(stageEntry.StartedAt, stageEntry.StoppedAt, nameof(stageEntry.StoppedAt));
  }

  private static TimeSpan GetDuration(TimeSpan from, TimeSpan to, string targetName)
  {
    return to < from
      ? throw new InvalidOperationException($"StageTracking duration is invalid: {targetName} is before source time.")
      : to - from;
  }

  private void PutOnHoldItem(OnHoldOccurrence onHoldOccurrence)
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
