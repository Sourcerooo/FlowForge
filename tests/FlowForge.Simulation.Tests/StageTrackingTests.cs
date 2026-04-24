using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.Entities.Stages;

namespace FlowForge.Simulation.Tests;

public sealed class StageTrackingTests
{
  private static readonly StageId TestStageId = StageId.NewId();
  private static readonly StationId TestStationId = StationId.NewId();

  [Fact]
  public void EnqueueWorkItem_FirstQueueIncrementsQueueCountersAndPeak()
  {
    var tracking = CreateTracking();

    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.FromSeconds(1)), OnQueueOccurrence.First);

    Assert.Equal(1, tracking.WorkItemsQueuedCount);
    Assert.Equal(1, tracking.CurrentQueueLength);
    Assert.Equal(1, tracking.PeakQueueLength);
  }

  [Fact]
  public void EnqueueWorkItem_MultipleTimesCountsAllInitialQueues()
  {
    var tracking = CreateTracking();

    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.FromSeconds(1)), OnQueueOccurrence.First);
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.FromSeconds(2)), OnQueueOccurrence.First);
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.FromSeconds(3)), OnQueueOccurrence.First);

    Assert.Equal(3, tracking.WorkItemsQueuedCount);
    Assert.Equal(3, tracking.CurrentQueueLength);
    Assert.Equal(3, tracking.PeakQueueLength);
  }

  [Fact]
  public void EnqueueWorkItem_RequeueTracksOnHoldTimeAndQueueCounters()
  {
    var tracking = CreateTracking();

    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.FromSeconds(1)), OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.FromSeconds(1), startedAt: TimeSpan.FromSeconds(6)),
      StageTracking.ProcessingKind.InitialStartFromQueue);

    tracking.PutOnHoldWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(6), stoppedAt: TimeSpan.FromSeconds(14)),
      OnHoldOccurrence.First);

    tracking.EnqueueWorkItem(
      CreateEntry(stoppedAt: TimeSpan.FromSeconds(14), requeuedAt: TimeSpan.FromSeconds(18)),
      OnQueueOccurrence.Requeued);

    Assert.Equal(1, tracking.WorkItemsQueuedCount);
    Assert.Equal(1, tracking.WorkItemsRequeuedCount);
    Assert.Equal(1, tracking.CurrentQueueLength);
    Assert.Equal(TimeSpan.FromSeconds(5), tracking.CumulativeQueueWait);
    Assert.Equal(TimeSpan.FromSeconds(8), tracking.CumulativeProcessingTime);
    Assert.Equal(TimeSpan.FromSeconds(4), tracking.CumulativeOnHoldTime);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
  }

  [Fact]
  public void EnqueueWorkItem_RequeueWithoutPriorHoldThrows()
  {
    var tracking = CreateTracking();

    void Action() => tracking.EnqueueWorkItem(
      CreateEntry(requeuedAt: TimeSpan.FromSeconds(18)),
      OnQueueOccurrence.Requeued);

    var exception = Assert.Throws<InvalidOperationException>(Action);
    Assert.Equal("StageTracking requeue requires a prior hold/stop timestamp.", exception.Message);
  }

  [Fact]
  public void StartProcessingWorkItem_InitialStartConsumesQueueWaitAndIncrementsStartedCount()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.Zero), OnQueueOccurrence.First);

    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.Zero, startedAt: TimeSpan.FromSeconds(12)),
      StageTracking.ProcessingKind.InitialStartFromQueue);

    Assert.Equal(1, tracking.WorkItemsStartedCount);
    Assert.Equal(TimeSpan.FromSeconds(12), tracking.CumulativeQueueWait);
    Assert.Equal(0, tracking.CurrentQueueLength);
    Assert.Equal(1, tracking.CurrentBusyWorkers);
    Assert.Equal(1, tracking.PeakBusyWorkers);
  }

  [Fact]
  public void StartProcessingWorkItem_ResumeFromQueueConsumesQueueWaitButDoesNotIncrementStartedCount()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.Zero), OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.Zero, startedAt: TimeSpan.FromSeconds(4)),
      StageTracking.ProcessingKind.InitialStartFromQueue);
    tracking.PutOnHoldWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(4), stoppedAt: TimeSpan.FromSeconds(10)),
      OnHoldOccurrence.First);
    tracking.EnqueueWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(4), stoppedAt: TimeSpan.FromSeconds(10), requeuedAt: TimeSpan.FromSeconds(10)),
      OnQueueOccurrence.Requeued);

    tracking.StartProcessingWorkItem(
      CreateEntry(requeuedAt: TimeSpan.FromSeconds(10), startedAt: TimeSpan.FromSeconds(19)),
      StageTracking.ProcessingKind.ResumeFromQueue);

    Assert.Equal(1, tracking.WorkItemsStartedCount);
    Assert.Equal(TimeSpan.FromSeconds(13), tracking.CumulativeQueueWait);
    Assert.Equal(TimeSpan.FromSeconds(6), tracking.CumulativeProcessingTime);
    Assert.Equal(TimeSpan.Zero, tracking.CumulativeOnHoldTime);
    Assert.Equal(0, tracking.CurrentQueueLength);
    Assert.Equal(1, tracking.CurrentBusyWorkers);
    Assert.Equal(1, tracking.PeakBusyWorkers);
  }

  [Fact]
  public void StartProcessingWorkItem_ResumeFromOnHoldAccumulatesOnHoldTime()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.Zero), OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.Zero, startedAt: TimeSpan.FromSeconds(3)),
      StageTracking.ProcessingKind.InitialStartFromQueue);
    tracking.PutOnHoldWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(3), stoppedAt: TimeSpan.FromSeconds(10)),
      OnHoldOccurrence.First);

    tracking.StartProcessingWorkItem(
      CreateEntry(stoppedAt: TimeSpan.FromSeconds(10), startedAt: TimeSpan.FromSeconds(17)),
      StageTracking.ProcessingKind.ResumeFromOnHold);

    Assert.Equal(TimeSpan.FromSeconds(3), tracking.CumulativeQueueWait);
    Assert.Equal(TimeSpan.FromSeconds(7), tracking.CumulativeProcessingTime);
    Assert.Equal(TimeSpan.FromSeconds(7), tracking.CumulativeOnHoldTime);
    Assert.Equal(0, tracking.CurrentQueueLength);
    Assert.Equal(1, tracking.CurrentBusyWorkers);
  }

  [Fact]
  public void StartProcessingWorkItem_TracksPeakBusyWorkersAcrossConcurrentStarts()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.Zero), OnQueueOccurrence.First);
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.FromSeconds(1)), OnQueueOccurrence.First);

    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.Zero, startedAt: TimeSpan.FromSeconds(1)),
      StageTracking.ProcessingKind.InitialStartFromQueue);
    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.FromSeconds(1), startedAt: TimeSpan.FromSeconds(3)),
      StageTracking.ProcessingKind.InitialStartFromQueue);

    Assert.Equal(2, tracking.WorkItemsStartedCount);
    Assert.Equal(TimeSpan.FromSeconds(3), tracking.CumulativeQueueWait);
    Assert.Equal(0, tracking.CurrentQueueLength);
    Assert.Equal(2, tracking.CurrentBusyWorkers);
    Assert.Equal(2, tracking.PeakBusyWorkers);
  }

  [Fact]
  public void CompleteWorkItem_IncrementsCompletedCountAndAddsProcessingTime()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.Zero), OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.Zero, startedAt: TimeSpan.FromSeconds(3)),
      StageTracking.ProcessingKind.InitialStartFromQueue);

    tracking.CompleteWorkItem(CreateEntry(startedAt: TimeSpan.FromSeconds(3), completedAt: TimeSpan.FromSeconds(17)));

    Assert.Equal(1, tracking.WorkItemsCompletedCount);
    Assert.Equal(TimeSpan.FromSeconds(14), tracking.CumulativeProcessingTime);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
  }

  [Fact]
  public void StopWorkItem_FirstOnHoldIncrementsTransactionAndUniqueCounts()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.Zero), OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.Zero, startedAt: TimeSpan.FromSeconds(2)),
      StageTracking.ProcessingKind.InitialStartFromQueue);

    tracking.PutOnHoldWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(2), stoppedAt: TimeSpan.FromSeconds(11)),
      OnHoldOccurrence.First);

    Assert.Equal(1, tracking.TransactionsOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsUniqueOnHoldCount);
    Assert.Equal(TimeSpan.FromSeconds(9), tracking.CumulativeProcessingTime);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
  }

  [Fact]
  public void StopWorkItem_RepeatedOnHoldIncrementsTransactionCountButNotUniqueCount()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.Zero), OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.Zero, startedAt: TimeSpan.FromSeconds(2)),
      StageTracking.ProcessingKind.InitialStartFromQueue);
    tracking.PutOnHoldWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(2), stoppedAt: TimeSpan.FromSeconds(4)),
      OnHoldOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(stoppedAt: TimeSpan.FromSeconds(4), startedAt: TimeSpan.FromSeconds(9)),
      StageTracking.ProcessingKind.ResumeFromOnHold);

    tracking.PutOnHoldWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(9), stoppedAt: TimeSpan.FromSeconds(15)),
      OnHoldOccurrence.Repeated);

    Assert.Equal(2, tracking.TransactionsOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsUniqueOnHoldCount);
    Assert.Equal(TimeSpan.FromSeconds(8), tracking.CumulativeProcessingTime);
    Assert.Equal(TimeSpan.FromSeconds(5), tracking.CumulativeOnHoldTime);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
  }

  [Fact]
  public void ResumeFromOnHold_ReactivatesBusyWorkerWithoutTouchingQueue()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.Zero), OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.Zero, startedAt: TimeSpan.FromSeconds(2)),
      StageTracking.ProcessingKind.InitialStartFromQueue);
    tracking.PutOnHoldWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(2), stoppedAt: TimeSpan.FromSeconds(6)),
      OnHoldOccurrence.First);

    tracking.StartProcessingWorkItem(
      CreateEntry(stoppedAt: TimeSpan.FromSeconds(6), startedAt: TimeSpan.FromSeconds(9)),
      StageTracking.ProcessingKind.ResumeFromOnHold);

    Assert.Equal(0, tracking.CurrentQueueLength);
    Assert.Equal(1, tracking.CurrentBusyWorkers);
    Assert.Equal(TimeSpan.FromSeconds(3), tracking.CumulativeOnHoldTime);
  }

  [Fact]
  public void StopAndRequeueWorkItem_FirstOnHoldAndRepeatedQueueUpdatesBothSetsOfCounters()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.Zero), OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.Zero, startedAt: TimeSpan.FromSeconds(1)),
      StageTracking.ProcessingKind.InitialStartFromQueue);

    tracking.StopAndRequeueWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(1), stoppedAt: TimeSpan.FromSeconds(10), requeuedAt: TimeSpan.FromSeconds(10)),
      OnHoldOccurrence.First);

    Assert.Equal(1, tracking.WorkItemsQueuedCount);
    Assert.Equal(1, tracking.TransactionsOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsUniqueOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsRequeuedCount);
    Assert.Equal(1, tracking.CurrentQueueLength);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
    Assert.Equal(TimeSpan.FromSeconds(9), tracking.CumulativeProcessingTime);
  }

  [Fact]
  public void MixedQueueAndOnHoldCycles_AccumulateAllRelevantDurationsAndCounters()
  {
    var tracking = CreateTracking();

    tracking.EnqueueWorkItem(CreateEntry(enqueuedAt: TimeSpan.Zero), OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(enqueuedAt: TimeSpan.Zero, startedAt: TimeSpan.FromSeconds(2)),
      StageTracking.ProcessingKind.InitialStartFromQueue);
    tracking.StopAndRequeueWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(2), stoppedAt: TimeSpan.FromSeconds(7), requeuedAt: TimeSpan.FromSeconds(7)),
      OnHoldOccurrence.First);
    tracking.StartProcessingWorkItem(
      CreateEntry(requeuedAt: TimeSpan.FromSeconds(7), startedAt: TimeSpan.FromSeconds(11)),
      StageTracking.ProcessingKind.ResumeFromQueue);
    tracking.PutOnHoldWorkItem(
      CreateEntry(startedAt: TimeSpan.FromSeconds(11), stoppedAt: TimeSpan.FromSeconds(17)),
      OnHoldOccurrence.Repeated);
    tracking.StartProcessingWorkItem(
      CreateEntry(stoppedAt: TimeSpan.FromSeconds(17), startedAt: TimeSpan.FromSeconds(20)),
      StageTracking.ProcessingKind.ResumeFromOnHold);
    tracking.CompleteWorkItem(CreateEntry(startedAt: TimeSpan.FromSeconds(20), completedAt: TimeSpan.FromSeconds(27)));

    Assert.Equal(1, tracking.WorkItemsQueuedCount);
    Assert.Equal(1, tracking.WorkItemsStartedCount);
    Assert.Equal(1, tracking.WorkItemsCompletedCount);
    Assert.Equal(2, tracking.TransactionsOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsUniqueOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsRequeuedCount);
    Assert.Equal(TimeSpan.FromSeconds(6), tracking.CumulativeQueueWait);
    Assert.Equal(TimeSpan.FromSeconds(18), tracking.CumulativeProcessingTime);
    Assert.Equal(TimeSpan.FromSeconds(3), tracking.CumulativeOnHoldTime);
    Assert.Equal(0, tracking.CurrentQueueLength);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
    Assert.Equal(1, tracking.PeakQueueLength);
    Assert.Equal(1, tracking.PeakBusyWorkers);
  }

  [Fact]
  public void Stations_IsExposedAsReadOnlyDictionary()
  {
    var tracking = CreateTracking();

    Assert.IsAssignableFrom<IReadOnlyDictionary<StationId, FlowForge.Simulation.Tracking.Entities.Stations.StationTracking>>(tracking.Stations);
    Assert.Single(tracking.Stations);
  }

  private static StageTracking CreateTracking()
  {
    return new StageTracking(
      TestStageId,
      new Dictionary<StationId, FlowForge.Simulation.Tracking.Entities.Stations.StationTracking>
      {
        [TestStationId] = new(TestStationId, TestStageId)
      });
  }

  private static StageEntry CreateEntry(
    TimeSpan enqueuedAt = default,
    TimeSpan startedAt = default,
    TimeSpan completedAt = default,
    TimeSpan stoppedAt = default,
    TimeSpan requeuedAt = default)
  {
    return new StageEntry(
      TrackingSubjectId.NewId(),
      enqueuedAt,
      startedAt,
      completedAt,
      stoppedAt,
      requeuedAt,
      TestStageId,
      TestStationId);
  }
}
