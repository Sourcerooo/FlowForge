using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.Scenarios.Entities;
using FlowForge.Simulation.Runtime.Services;

namespace FlowForge.Simulation.Tests;

public sealed class StageRuntimeStateStoreTests
{
  [Fact]
  public void Enqueue_AndDequeue_WorkForKnownStage()
  {
    var stageId = StageId.NewId();
    var trackingSubjectId = TrackingSubjectId.NewId();
    var store = CreateStore(1, stageId);

    store.Enqueue(stageId, trackingSubjectId, TimeSpan.FromMinutes(3));

    var dequeued = store.Dequeue(stageId);

    Assert.NotNull(dequeued);
    Assert.Equal(trackingSubjectId, dequeued.Value.TrackingSubjectId);
    Assert.Equal(TimeSpan.FromMinutes(3), dequeued.Value.EnqueuedAt);
  }

  [Fact]
  public void Dequeue_ReturnsNullForUnknownStage()
  {
    var store = CreateStore(1);

    var dequeued = store.Dequeue(StageId.NewId());

    Assert.Null(dequeued);
  }

  [Fact]
  public void Enqueue_UnknownStage_DoesNothing()
  {
    var stageId = StageId.NewId();
    var store = CreateStore(1, stageId);

    store.Enqueue(StageId.NewId(), TrackingSubjectId.NewId(), TimeSpan.Zero);

    void Act() => store.Dequeue(stageId);

    Assert.Throws<InvalidOperationException>(Act);
  }

  [Fact]
  public void IsBusy_ReturnsFalseWhenKnownStageHasFreeCapacity()
  {
    var stageId = StageId.NewId();
    var store = CreateStore(1, stageId);

    var result = store.IsBusy(stageId);

    Assert.False(result);
  }

  [Fact]
  public void IsBusy_ReturnsTrueForUnknownStage()
  {
    var store = CreateStore(1);

    var result = store.IsBusy(StageId.NewId());

    Assert.True(result);
  }

  [Fact]
  public void TryStartProcessing_ReturnsTrueForKnownStageWithQueuedWorkAndFreeWorker()
  {
    var stageId = StageId.NewId();
    var trackingSubjectId = TrackingSubjectId.NewId();
    var store = CreateStore(1, stageId);
    store.Enqueue(stageId, trackingSubjectId, TimeSpan.FromSeconds(1));

    var result = store.TryStartProcessing(stageId, TimeSpan.FromSeconds(2));

    Assert.True(result.IsSuccess);
    Assert.True(store.IsBusy(stageId));
  }

  [Fact]
  public void TryStartProcessing_ReturnsFalseForUnknownStage()
  {
    var store = CreateStore(1);

    var result = store.TryStartProcessing(StageId.NewId(), TimeSpan.FromSeconds(1));
    Assert.True(result.IsFailure);
  }

  [Fact]
  public void TryFinishProcessing_ReturnsTrueForKnownProcessingWorkItem()
  {
    var stageId = StageId.NewId();
    var trackingSubjectId = TrackingSubjectId.NewId();
    var store = CreateStore(1, stageId);
    store.Enqueue(stageId, trackingSubjectId, TimeSpan.Zero);
    store.TryStartProcessing(stageId, TimeSpan.FromSeconds(1));

    store.CompleteProcessing(stageId, trackingSubjectId, TimeSpan.FromSeconds(5));

    Assert.False(store.IsBusy(stageId));
  }

  [Fact]
  public void TryFinishProcessing_ReturnsFalseForUnknownStage()
  {
    var store = CreateStore(1);
    var result = store.CompleteProcessing(StageId.NewId(), TrackingSubjectId.NewId(), TimeSpan.FromSeconds(5));
    Assert.True(result.IsFailure);
  }

  private static StageRuntimeStateStore CreateStore(int workerCapacity, StageId? stageId = null)
  {
    var stageDefinitions = new List<StageDefinition>();

    var newStageId = stageId ?? StageId.NewId();
    stageDefinitions.Add(new StageDefinition(
      newStageId,
      $"stage-1",
      $"Stage 1",
      1,
      new List<StationDefinition>
      {
          new StationDefinition(StationId.NewId(), newStageId, $"station-1-1", $"Station 1-1", workerCapacity, TimeSpan.FromMinutes(5))
      }));

    var processConfiguration = ProcessConfiguration.Create(
          "test-process", "Test Process", DateTime.UtcNow, TimeSpan.FromHours(2),
          new ArrivalProfileDefinition(TimeSpan.FromMinutes(60), 10, 10),
          stageDefinitions);

    return new StageRuntimeStateStore(processConfiguration);
  }
}
