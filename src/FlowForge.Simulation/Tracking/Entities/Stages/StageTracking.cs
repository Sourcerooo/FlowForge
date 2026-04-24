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
  public StageTracking(StageId stageId, IReadOnlyDictionary<StationId, StationTracking> stations)
  {
    StageId = stageId;
    Stations = stations;
  }

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
    ValidateEnqueue(stageEntry, onQueueOccurrence);
    EnqueueItem(onQueueOccurrence);
    if (onQueueOccurrence == OnQueueOccurrence.Requeued)
    {
      var onHoldTime = GetDuration(stageEntry.StoppedAt, stageEntry.RequeuedAt, nameof(stageEntry.RequeuedAt));
      GetStationTracking(stageEntry).RequeueWorkItem(onHoldTime);
      CumulativeOnHoldTime += onHoldTime;
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
        GetStationTracking(stageEntry).StartWorkItem();
        WorkItemsStartedCount++;
        CumulativeQueueWait += GetDuration(stageEntry.EnqueuedAt, stageEntry.StartedAt, nameof(stageEntry.StartedAt));
        CurrentQueueLength--;
        break;
      case ProcessingKind.ResumeFromQueue:
        GetStationTracking(stageEntry).StartWorkItem();
        CumulativeQueueWait += GetDuration(stageEntry.RequeuedAt, stageEntry.StartedAt, nameof(stageEntry.StartedAt));
        CurrentQueueLength--;
        break;
      case ProcessingKind.ResumeFromOnHold:
        var onHoldTime = GetDuration(stageEntry.StoppedAt, stageEntry.StartedAt, nameof(stageEntry.StartedAt));
        GetStationTracking(stageEntry).ResumeWorkItem(onHoldTime);
        CumulativeOnHoldTime += onHoldTime;
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
    var processingTime = GetDuration(stageEntry.StartedAt, stageEntry.CompletedAt, nameof(stageEntry.CompletedAt));
    GetStationTracking(stageEntry).CompleteWorkItem(processingTime);
    CumulativeProcessingTime += processingTime;
    WorkItemsCompletedCount++;
    CurrentBusyWorkers--;
  }


  public void PutOnHoldWorkItem(StageEntry stageEntry, OnHoldOccurrence onHoldOccurrence)
  {
    PutOnHoldItem(onHoldOccurrence);
    var processingTime = GetDuration(stageEntry.StartedAt, stageEntry.StoppedAt, nameof(stageEntry.StoppedAt));
    GetStationTracking(stageEntry).PutOnHoldWorkItem(processingTime);
    CumulativeProcessingTime += processingTime;
  }

  public void StopAndRequeueWorkItem(StageEntry stageEntry, OnHoldOccurrence onHoldOccurrence)
  {
    PutOnHoldItem(onHoldOccurrence);
    EnqueueItem(OnQueueOccurrence.Requeued);
    var processingTime = GetDuration(stageEntry.StartedAt, stageEntry.StoppedAt, nameof(stageEntry.StoppedAt));
    var stationTracking = GetStationTracking(stageEntry);
    stationTracking.PutOnHoldWorkItem(processingTime);
    stationTracking.RequeueWorkItem(TimeSpan.FromSeconds(0));
    CumulativeProcessingTime += GetDuration(stageEntry.StartedAt, stageEntry.StoppedAt, nameof(stageEntry.StoppedAt));
  }

  private static TimeSpan GetDuration(TimeSpan from, TimeSpan to, string targetName)
  {
    return to < from
      ? throw new InvalidOperationException($"StageTracking duration is invalid: {targetName} is before source time.")
      : to - from;
  }

  private static void ValidateEnqueue(StageEntry stageEntry, OnQueueOccurrence onQueueOccurrence)
  {
    if (onQueueOccurrence != OnQueueOccurrence.Requeued)
    {
      return;
    }

    if (stageEntry.StoppedAt == default)
    {
      throw new InvalidOperationException("StageTracking requeue requires a prior hold/stop timestamp.");
    }

    if (stageEntry.RequeuedAt == default)
    {
      throw new InvalidOperationException("StageTracking requeue requires a requeue timestamp.");
    }
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

  private StationTracking GetStationTracking(StageEntry stageEntry)
  {
    return stageEntry.StationId.HasValue
      ? !Stations.TryGetValue(stageEntry.StationId.Value, out var stationTracking)
        ? throw new InvalidDataException($"Tracking with StationId {stageEntry.StationId.Value} does not exist")
        : stationTracking
      : throw new InvalidDataException($"Tracking has no StationId assigned");
  }
}
