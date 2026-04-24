using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.Entities.WorkItems;
using FlowForge.Simulation.Tracking.Enums;

namespace FlowForge.Simulation.Tests;

public sealed class WorkItemTrackingTests
{
  [Fact]
  public void CompleteProcessingWorkItem_EndsCurrentSegmentWithoutSettingCompletedAt()
  {
    var tracking = CreateProcessingTracking(out _);

    tracking.CompleteProcessingWorkItem(TimeSpan.FromSeconds(12));

    Assert.Equal(2, tracking.Segments.Count);
    Assert.Equal(TrackingSegmentType.QueueWait, tracking.Segments[0].SegmentType);
    var segment = tracking.Segments[1];
    Assert.Equal(TrackingSegmentType.Processing, segment.SegmentType);
    Assert.Equal(TimeSpan.FromSeconds(10), segment.StartedAt);
    Assert.Equal(TimeSpan.FromSeconds(12), segment.EndedAt);
    Assert.Null(tracking.CompletedAt);
    Assert.Null(tracking.TotalLeadTime);
  }

  [Fact]
  public void CompleteProcessingWorkItem_AllowsOpeningNextSegmentForFollowingStage()
  {
    var tracking = CreateProcessingTracking(out var workItem);

    tracking.CompleteProcessingWorkItem(TimeSpan.FromSeconds(12));
    workItem.QueueForStage(StageId.NewId(), workItem.CurrentProcessingToken);
    tracking.EnqueueWorkItem(workItem, TimeSpan.FromSeconds(15));

    Assert.Equal(3, tracking.Segments.Count);
    Assert.Equal(TrackingSegmentType.QueueWait, tracking.Segments[0].SegmentType);
    Assert.Equal(TrackingSegmentType.Processing, tracking.Segments[1].SegmentType);
    Assert.Equal(TimeSpan.FromSeconds(12), tracking.Segments[1].EndedAt);
    Assert.Equal(TrackingSegmentType.QueueWait, tracking.Segments[2].SegmentType);
    Assert.Equal(TimeSpan.FromSeconds(15), tracking.Segments[2].StartedAt);
    Assert.Null(tracking.CompletedAt);
  }

  [Fact]
  public void CompleteWorkItem_EndsCurrentSegmentAndSetsCompletionMetadata()
  {
    var tracking = CreateProcessingTracking(out _);

    tracking.CompleteWorkItem(TimeSpan.FromSeconds(18));

    Assert.Equal(2, tracking.Segments.Count);
    var segment = tracking.Segments[1];
    Assert.Equal(TimeSpan.FromSeconds(18), segment.EndedAt);
    Assert.Equal(TimeSpan.FromSeconds(18), tracking.CompletedAt);
    Assert.Equal(TimeSpan.FromSeconds(13), tracking.TotalLeadTime);
  }

  [Fact]
  public void CompleteProcessingWorkItem_WhenCompletionTimeIsBeforeSegmentStart_Throws()
  {
    var tracking = CreateProcessingTracking(out _);

    void Act() => tracking.CompleteProcessingWorkItem(TimeSpan.FromSeconds(9));

    var exception = Assert.Throws<InvalidOperationException>(Act);
    Assert.Equal($"WorkItemTracking for item: {tracking.TrackingSubjectId}. CompletionTime before StartTime", exception.Message);
  }

  private static WorkItemTracking CreateProcessingTracking(out WorkItemRuntimeState workItem)
  {
    var trackingSubjectId = TrackingSubjectId.NewId();
    var stageId = StageId.NewId();
    var stationId = StationId.NewId();
    workItem = new WorkItemRuntimeState(
      trackingSubjectId,
      TimeSpan.FromSeconds(5),
      WorkItemStatus.Created,
      null,
      null,
      new ProcessingToken(3));

    var tracking = new WorkItemTracking(trackingSubjectId, TimeSpan.FromSeconds(5));

    workItem.QueueForStage(stageId, workItem.CurrentProcessingToken);
    tracking.EnqueueWorkItem(workItem, TimeSpan.FromSeconds(7));
    workItem.StartProcessing(stationId);
    tracking.ProcessWorkItem(workItem, TimeSpan.FromSeconds(10));

    return tracking;
  }
}
