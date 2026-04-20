using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Tracking.Entities.Stages;

namespace FlowForge.Simulation.Tests;

public sealed class StageTrackingTests
{
  [Fact]
  public void EnqueueWorkItem_FirstQueueIncrementsQueueCountersAndPeak()
  {
    var tracking = CreateTracking();

    tracking.EnqueueWorkItem(OnQueueOccurrence.First);

    Assert.Equal(1, tracking.WorkItemsQueuedCount);
    Assert.Equal(1, tracking.CurrentQueueLength);
    Assert.Equal(1, tracking.PeakQueueLength);
  }

  [Fact]
  public void EnqueueWorkItem_MultipleTimesOnlyCountsFirstQueueButTracksCurrentAndPeakLength()
  {
    var tracking = CreateTracking();

    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);

    Assert.Equal(3, tracking.WorkItemsQueuedCount);
    Assert.Equal(3, tracking.CurrentQueueLength);
    Assert.Equal(3, tracking.PeakQueueLength);
  }

  [Fact]
  public void RequeueWorkItem_IncrementsQueueLengthWithoutIncreasingUniqueQueuedCount()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(5));

    tracking.EnqueueWorkItem(OnQueueOccurrence.Requeued, TimeSpan.FromSeconds(8));

    Assert.Equal(1, tracking.WorkItemsQueuedCount);
    Assert.Equal(1, tracking.WorkItemsRequeuedCount);
    Assert.Equal(1, tracking.CurrentQueueLength);
    Assert.Equal(TimeSpan.FromSeconds(8), tracking.CumulativeProcessingTime);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
  }

  [Fact]
  public void StartProcessingWorkItem_InitialStartConsumesQueueWaitAndIncrementsStartedCount()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);

    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(12));

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
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(4));
    tracking.EnqueueWorkItem(OnQueueOccurrence.Requeued, TimeSpan.FromSeconds(6));

    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.ResumeFromQueue, TimeSpan.FromSeconds(9));

    Assert.Equal(1, tracking.WorkItemsStartedCount);
    Assert.Equal(TimeSpan.FromSeconds(13), tracking.CumulativeQueueWait);
    Assert.Equal(0, tracking.CurrentQueueLength);
    Assert.Equal(1, tracking.CurrentBusyWorkers);
    Assert.Equal(1, tracking.PeakBusyWorkers);
  }

  [Fact]
  public void StartProcessingWorkItem_ResumeFromOnHoldAccumulatesOnHoldTimeWithoutChangingQueueMetrics()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(3));
    tracking.StopWorkItem(TimeSpan.FromSeconds(10), OnHoldOccurrence.First);

    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.ResumeFromOnHold, onHoldTime: TimeSpan.FromSeconds(7));

    Assert.Equal(TimeSpan.FromSeconds(3), tracking.CumulativeQueueWait);
    Assert.Equal(TimeSpan.FromSeconds(7), tracking.CumulativeOnHoldTime);
    Assert.Equal(0, tracking.CurrentQueueLength);
    Assert.Equal(1, tracking.CurrentBusyWorkers);
  }

  [Fact]
  public void StartProcessingWorkItem_TracksPeakBusyWorkersAcrossConcurrentStarts()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);

    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(1));
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(2));

    Assert.Equal(2, tracking.WorkItemsStartedCount);
    Assert.Equal(0, tracking.CurrentQueueLength);
    Assert.Equal(2, tracking.CurrentBusyWorkers);
    Assert.Equal(2, tracking.PeakBusyWorkers);
  }

  [Fact]
  public void CompleteWorkItem_IncrementsCompletedCountAndAddsProcessingTime()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(3));

    tracking.CompleteWorkItem(TimeSpan.FromSeconds(14));

    Assert.Equal(1, tracking.WorkItemsCompletedCount);
    Assert.Equal(TimeSpan.FromSeconds(14), tracking.CumulativeProcessingTime);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
  }

  [Fact]
  public void StopWorkItem_FirstOnHoldIncrementsTransactionAndUniqueCounts()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(2));

    tracking.StopWorkItem(TimeSpan.FromSeconds(11), OnHoldOccurrence.First);

    Assert.Equal(1, tracking.TransactionsOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsUniqueOnHoldCount);
    Assert.Equal(TimeSpan.FromSeconds(11), tracking.CumulativeProcessingTime);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
  }

  [Fact]
  public void StopWorkItem_RepeatedOnHoldIncrementsTransactionCountButNotUniqueCount()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(2));
    tracking.StopWorkItem(TimeSpan.FromSeconds(4), OnHoldOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.ResumeFromOnHold, onHoldTime: TimeSpan.FromSeconds(5));

    tracking.StopWorkItem(TimeSpan.FromSeconds(6), OnHoldOccurrence.Repeated);

    Assert.Equal(2, tracking.TransactionsOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsUniqueOnHoldCount);
    Assert.Equal(TimeSpan.FromSeconds(10), tracking.CumulativeProcessingTime);
    Assert.Equal(TimeSpan.FromSeconds(5), tracking.CumulativeOnHoldTime);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
  }

  [Fact]
  public void StopAndRequeueWorkItem_FirstOnHoldAndRepeatedQueueUpdatesBothSetsOfCounters()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(1));

    tracking.StopAndRequeueWorkItem(TimeSpan.FromSeconds(9), OnHoldOccurrence.First);

    Assert.Equal(1, tracking.WorkItemsQueuedCount);
    Assert.Equal(1, tracking.TransactionsOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsUniqueOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsRequeuedCount);
    Assert.Equal(1, tracking.CurrentQueueLength);
    Assert.Equal(0, tracking.CurrentBusyWorkers);
    Assert.Equal(TimeSpan.FromSeconds(9), tracking.CumulativeProcessingTime);
  }

  [Fact]
  public void StopAndRequeueWorkItem_RepeatedOnHoldAndFirstQueueIncrementsUniqueQueueCount()
  {
    var tracking = CreateTracking();
    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(1));
    tracking.StopWorkItem(TimeSpan.FromSeconds(3), OnHoldOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.ResumeFromOnHold, onHoldTime: TimeSpan.FromSeconds(2));

    tracking.StopAndRequeueWorkItem(TimeSpan.FromSeconds(7), OnHoldOccurrence.Repeated);

    Assert.Equal(1, tracking.WorkItemsQueuedCount);
    Assert.Equal(2, tracking.TransactionsOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsUniqueOnHoldCount);
    Assert.Equal(1, tracking.WorkItemsRequeuedCount);
    Assert.Equal(1, tracking.CurrentQueueLength);
    Assert.Equal(TimeSpan.FromSeconds(10), tracking.CumulativeProcessingTime);
  }

  [Fact]
  public void MixedQueueAndOnHoldCycles_AccumulateAllRelevantDurationsAndCounters()
  {
    var tracking = CreateTracking();

    tracking.EnqueueWorkItem(OnQueueOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.InitialStartFromQueue, TimeSpan.FromSeconds(2));
    tracking.StopAndRequeueWorkItem(TimeSpan.FromSeconds(5), OnHoldOccurrence.First);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.ResumeFromQueue, TimeSpan.FromSeconds(4));
    tracking.StopWorkItem(TimeSpan.FromSeconds(6), OnHoldOccurrence.Repeated);
    tracking.StartProcessingWorkItem(StageTracking.ProcessingKind.ResumeFromOnHold, onHoldTime: TimeSpan.FromSeconds(3));
    tracking.CompleteWorkItem(TimeSpan.FromSeconds(7));

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
    Assert.Empty(tracking.Stations);
  }

  private static StageTracking CreateTracking()
  {
    return new StageTracking
    {
      StageId = StageId.NewId()
    };
  }
}
