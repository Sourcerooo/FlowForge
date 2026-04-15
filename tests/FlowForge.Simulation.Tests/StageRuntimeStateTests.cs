using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Tests;

public sealed class StageRuntimeStateTests
{
  [Fact]
  public void Enqueue_AndDequeue_PreserveFifoOrder()
  {
    var stage = CreateStage(workerCapacities: [1]);
    var first = NewQueueEntry(TimeSpan.FromMinutes(1));
    var second = NewQueueEntry(TimeSpan.FromMinutes(2));

    stage.Enqueue(first);
    stage.Enqueue(second);

    Assert.Equal(first, stage.Dequeue());
    Assert.Equal(second, stage.Dequeue());
  }

  [Fact]
  public void Dequeue_WhenQueueIsEmpty_ThrowsInvalidOperationException()
  {
    var stage = CreateStage(workerCapacities: [1]);

    void Act() => stage.Dequeue();

    Assert.Throws<InvalidOperationException>(Act);
  }

  [Fact]
  public void IsBusy_ReturnsFalseWhenAnyStationHasCapacity()
  {
    var stage = CreateStage(workerCapacities: [1, 1]);

    Assert.False(stage.IsBusy());
  }

  [Fact]
  public void IsBusy_ReturnsTrueWhenAllStationsAreAtCapacity()
  {
    var stage = CreateStage(workerCapacities: [1, 1]);
    stage.Enqueue(NewQueueEntry(TimeSpan.Zero));
    stage.Enqueue(NewQueueEntry(TimeSpan.FromSeconds(1)));
    stage.TryStartProcessing(TimeSpan.FromSeconds(1));
    stage.TryStartProcessing(TimeSpan.FromSeconds(2));

    Assert.True(stage.IsBusy());
  }

  [Fact]
  public void TryStartProcessing_ReturnsFalseWhenStageHasCapacityButQueueIsEmpty()
  {
    var stage = CreateStage(workerCapacities: [1]);

    var result = stage.TryStartProcessing(TimeSpan.FromMinutes(1));

    Assert.False(result.IsSuccess);
  }

  [Fact]
  public void TryStartProcessing_DequeuesOldestEntryAndReservesAWorker()
  {
    var stage = CreateStage(workerCapacities: [1]);
    var entry = NewQueueEntry(TimeSpan.FromMinutes(1));
    entry = entry with { ProcessingToken = 9 };
    stage.Enqueue(entry);

    var result = stage.TryStartProcessing(TimeSpan.FromMinutes(2));

    Assert.True(result.IsSuccess);
    Assert.Empty(stage.Queue);
    var station = Assert.Single(stage.Stations).Value;
    var processing = Assert.Single(station.ProcessingInfo);
    Assert.Equal(entry.TrackingSubjectId, processing.Value.TrackingSubjectId);
    Assert.Equal(9, processing.Value.ProcessingToken);
  }

  [Fact]
  public void TryStartProcessing_WithSingleStation_UsesThatStation()
  {
    var stage = CreateStage(workerCapacities: [2]);
    stage.Enqueue(NewQueueEntry(TimeSpan.Zero));

    var result = stage.TryStartProcessing(TimeSpan.FromMinutes(1));

    Assert.True(result.IsSuccess);
    var station = Assert.Single(stage.Stations).Value;
    Assert.Equal(1, station.BusyWorkerCount);
  }

  [Fact]
  public void TryStartProcessing_UsesRoundRobinAcrossStations()
  {
    var stage = CreateStage(workerCapacities: [1, 1, 1]);
    var entries = new[]
    {
      NewQueueEntry(TimeSpan.FromSeconds(1)),
      NewQueueEntry(TimeSpan.FromSeconds(2)),
      NewQueueEntry(TimeSpan.FromSeconds(3))
    };

    foreach (var entry in entries)
    {
      stage.Enqueue(entry);
      Assert.True(stage.TryStartProcessing(entry.EnqueuedAt + TimeSpan.FromSeconds(1)).IsSuccess);
    }

    var stations = stage.Stations.Values.ToList();
    Assert.All(stations, station => Assert.Equal(1, station.BusyWorkerCount));
  }

  [Fact]
  public void TryStartProcessing_ReturnsFalseWhenAllStationsAreBusy_AndKeepsQueueUntouched()
  {
    var stage = CreateStage(workerCapacities: [1]);
    var first = NewQueueEntry(TimeSpan.Zero);
    var second = NewQueueEntry(TimeSpan.FromSeconds(2));
    stage.Enqueue(first);
    stage.Enqueue(second);
    stage.TryStartProcessing(TimeSpan.FromSeconds(1));

    var result = stage.TryStartProcessing(TimeSpan.FromSeconds(3));

    Assert.False(result.IsSuccess);
    var remaining = Assert.Single(stage.Queue);
    Assert.Equal(second, remaining);
  }

  [Fact]
  public void TryFinishProcessing_ReleasesAssignedStation()
  {
    var stage = CreateStage(workerCapacities: [1]);
    var entry = NewQueueEntry(TimeSpan.Zero);
    stage.Enqueue(entry);
    stage.TryStartProcessing(TimeSpan.FromSeconds(1));

    stage.CompleteProcessing(entry.TrackingSubjectId);

    var station = Assert.Single(stage.Stations).Value;
    Assert.Equal(0, station.BusyWorkerCount);
    Assert.Empty(station.ProcessingInfo);
  }

  [Fact]
  public void TryFinishProcessing_ThrowsForUnknownTrackingSubjectId()
  {
    var stage = CreateStage(workerCapacities: [1]);

    void Act() => stage.CompleteProcessing(TrackingSubjectId.NewId());

    Assert.Throws<InvalidOperationException>(Act);
  }

  [Fact]
  public void Queue_IsExposedAsReadOnlyCollection()
  {
    var stage = CreateStage(workerCapacities: [1]);

    Assert.IsAssignableFrom<IReadOnlyCollection<StageQueueEntry>>(stage.Queue);
  }

  private static StageRuntimeState CreateStage(int[] workerCapacities)
  {
    var stageId = StageId.NewId();
    var stations = workerCapacities
      .Select(capacity => new StationRuntimeState(StationId.NewId(), stageId, capacity))
      .ToDictionary(station => station.StationId, station => station);

    return new StageRuntimeState(stageId, stations);
  }

  private static StageQueueEntry NewQueueEntry(TimeSpan enqueuedAt)
  {
    return new StageQueueEntry(TrackingSubjectId.NewId(), enqueuedAt);
  }
}
