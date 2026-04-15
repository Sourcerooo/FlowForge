using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Services;
using System.Collections.Immutable;

namespace FlowForge.Simulation.Tests;

public sealed class StageRuntimeStateStoreTests
{
  [Fact]
  public void Enqueue_AndDequeue_WorkForKnownStage()
  {
    var stageId = StageId.NewId();
    var trackingSubjectId = TrackingSubjectId.NewId();
    var store = CreateStore((stageId, [1]));

    store.Enqueue(stageId, trackingSubjectId, TimeSpan.FromMinutes(3));

    var dequeued = store.Dequeue(stageId);

    Assert.NotNull(dequeued);
    Assert.Equal(trackingSubjectId, dequeued.Value.TrackingSubjectId);
    Assert.Equal(TimeSpan.FromMinutes(3), dequeued.Value.EnqueuedAt);
  }

  [Fact]
  public void Dequeue_ReturnsNullForUnknownStage()
  {
    var store = CreateStore((StageId.NewId(), [1]));

    var dequeued = store.Dequeue(StageId.NewId());

    Assert.Null(dequeued);
  }

  [Fact]
  public void Enqueue_UnknownStage_DoesNothing()
  {
    var stageId = StageId.NewId();
    var store = CreateStore((stageId, [1]));

    store.Enqueue(StageId.NewId(), TrackingSubjectId.NewId(), TimeSpan.Zero);

    void Act() => store.Dequeue(stageId);

    Assert.Throws<InvalidOperationException>(Act);
  }

  [Fact]
  public void IsBusy_ReturnsFalseWhenKnownStageHasFreeCapacity()
  {
    var stageId = StageId.NewId();
    var store = CreateStore((stageId, [1]));

    var result = store.IsBusy(stageId);

    Assert.False(result);
  }

  [Fact]
  public void IsBusy_ReturnsTrueForUnknownStage()
  {
    var store = CreateStore((StageId.NewId(), [1]));

    var result = store.IsBusy(StageId.NewId());

    Assert.True(result);
  }

  [Fact]
  public void TryStartProcessing_ReturnsTrueForKnownStageWithQueuedWorkAndFreeWorker()
  {
    var stageId = StageId.NewId();
    var trackingSubjectId = TrackingSubjectId.NewId();
    var store = CreateStore((stageId, [1]));
    store.Enqueue(stageId, trackingSubjectId, TimeSpan.FromSeconds(1));

    var result = store.TryStartProcessing(stageId, TimeSpan.FromSeconds(2));

    Assert.True(result.IsSuccess);
    Assert.True(store.IsBusy(stageId));
  }

  [Fact]
  public void TryStartProcessing_ReturnsFalseForUnknownStage()
  {
    var store = CreateStore((StageId.NewId(), [1]));

    void Act() => store.TryStartProcessing(StageId.NewId(), TimeSpan.FromSeconds(1));

    Assert.Throws<InvalidOperationException>(Act);
  }

  [Fact]
  public void TryFinishProcessing_ReturnsTrueForKnownProcessingWorkItem()
  {
    var stageId = StageId.NewId();
    var trackingSubjectId = TrackingSubjectId.NewId();
    var store = CreateStore((stageId, [1]));
    store.Enqueue(stageId, trackingSubjectId, TimeSpan.Zero);
    store.TryStartProcessing(stageId, TimeSpan.FromSeconds(1));

    store.CompleteProcessing(stageId, trackingSubjectId);

    Assert.False(store.IsBusy(stageId));
  }

  [Fact]
  public void TryFinishProcessing_ReturnsFalseForUnknownStage()
  {
    var store = CreateStore((StageId.NewId(), [1]));
    void Act() => store.CompleteProcessing(StageId.NewId(), TrackingSubjectId.NewId());
    Assert.Throws<InvalidOperationException>(Act);
  }

  private static StageRuntimeStateStore CreateStore(params (StageId StageId, int[] WorkerCapacities)[] stages)
  {
    var definitions = stages.Select((stage, stageIndex) => new StageDefinition(
      stage.StageId,
      $"stage-{stageIndex + 1}",
      $"Stage {stageIndex + 1}",
      stageIndex,
      stage.WorkerCapacities
        .Select((capacity, stationIndex) => new StationDefinition(
          StationId.NewId(),
          stage.StageId,
          $"station-{stageIndex + 1}-{stationIndex + 1}",
          $"Station {stationIndex + 1}",
          capacity,
          TimeSpan.Zero))
        .ToList()));

    return new StageRuntimeStateStore(definitions.ToImmutableList());
  }
}
