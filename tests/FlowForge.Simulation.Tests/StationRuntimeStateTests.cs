using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Tests;

public sealed class StationRuntimeStateTests
{
  [Fact]
  public void Ctor_InitializesWithAllWorkersFree()
  {
    var station = CreateStation(workerCapacity: 3);

    Assert.Equal(3, station.WorkerCapacity);
    Assert.Equal(0, station.BusyWorkerCount);
    Assert.Equal(3, station.AvailableWorkerCount);
    Assert.True(station.HasFreeWorker);
    Assert.Empty(station.ProcessingInfo);
  }

  [Fact]
  public void TryReserveWorker_AssignsFirstFreeSlotAndStoresProcessingInfo()
  {
    var station = CreateStation(workerCapacity: 2);
    var trackingSubjectId = TrackingSubjectId.NewId();
    var startedAt = TimeSpan.FromMinutes(5);

    var result = station.TryReserveWorker(trackingSubjectId, startedAt, processingToken: new ProcessingToken(7));

    Assert.True(result);
    Assert.Equal(1, station.BusyWorkerCount);
    Assert.Equal(1, station.AvailableWorkerCount);
    var processing = Assert.Single(station.ProcessingInfo);
    Assert.Equal(0, processing.Key);
    Assert.Equal(trackingSubjectId, processing.Value.TrackingSubjectId);
    Assert.Equal(startedAt, processing.Value.StartedAt);
    Assert.Equal(7, processing.Value.ProcessingToken.Value);
  }

  [Fact]
  public void TryReserveWorker_UsesNextFreeSlotWhenEarlierSlotIsBusy()
  {
    var station = CreateStation(workerCapacity: 2);

    station.TryReserveWorker(TrackingSubjectId.NewId(), TimeSpan.FromMinutes(1), processingToken: new ProcessingToken(1));
    var secondTrackingId = TrackingSubjectId.NewId();

    var result = station.TryReserveWorker(secondTrackingId, TimeSpan.FromMinutes(2), processingToken: new ProcessingToken(2));

    Assert.True(result);
    Assert.Equal(2, station.BusyWorkerCount);
    Assert.True(station.ProcessingInfo.ContainsKey(1));
    Assert.Equal(secondTrackingId, station.ProcessingInfo[1].TrackingSubjectId);
  }

  [Fact]
  public void TryReserveWorker_ReturnsFalseWhenAllWorkersAreBusy()
  {
    var station = CreateStation(workerCapacity: 1);
    station.TryReserveWorker(TrackingSubjectId.NewId(), TimeSpan.Zero, processingToken: new ProcessingToken(1));

    var result = station.TryReserveWorker(TrackingSubjectId.NewId(), TimeSpan.FromSeconds(1), processingToken: new ProcessingToken(2));

    Assert.False(result);
    Assert.Equal(1, station.BusyWorkerCount);
    Assert.False(station.HasFreeWorker);
  }

  [Fact]
  public void TryReserveWorker_WithZeroCapacity_ReturnsFalse()
  {
    var station = CreateStation(workerCapacity: 0);

    var result = station.TryReserveWorker(TrackingSubjectId.NewId(), TimeSpan.Zero, processingToken: new ProcessingToken(1));

    Assert.False(result);
    Assert.Equal(0, station.BusyWorkerCount);
    Assert.False(station.HasFreeWorker);
  }

  [Fact]
  public void ReleaseWorker_FreesCorrectSlotAndPreservesOtherAssignments()
  {
    var station = CreateStation(workerCapacity: 3);
    var first = TrackingSubjectId.NewId();
    var second = TrackingSubjectId.NewId();

    station.TryReserveWorker(first, TimeSpan.FromMinutes(1), processingToken: new Runtime.ValueObjects.ProcessingToken(1));
    station.TryReserveWorker(second, TimeSpan.FromMinutes(2), processingToken: new Runtime.ValueObjects.ProcessingToken(2));

    station.ReleaseWorker(first);

    Assert.Equal(1, station.BusyWorkerCount);
    Assert.False(station.ProcessingInfo.ContainsKey(0));
    Assert.True(station.ProcessingInfo.ContainsKey(1));
    Assert.Equal(second, station.ProcessingInfo[1].TrackingSubjectId);
    Assert.True(station.HasFreeWorker);
  }

  [Fact]
  public void ReleaseWorker_ThrowsForUnknownTrackingSubjectId()
  {
    var station = CreateStation(workerCapacity: 1);

    void Act() => station.ReleaseWorker(TrackingSubjectId.NewId());

    var exception = Assert.Throws<ArgumentException>(Act);
    Assert.Contains("No worker found", exception.Message);
  }

  [Fact]
  public void ReleaseWorker_AfterFreeingSlot_AllowsNewReservationInSameSlot()
  {
    var station = CreateStation(workerCapacity: 1);
    var first = TrackingSubjectId.NewId();
    var second = TrackingSubjectId.NewId();

    station.TryReserveWorker(first, TimeSpan.FromMinutes(1), processingToken: new ProcessingToken(1));
    station.ReleaseWorker(first);
    var result = station.TryReserveWorker(second, TimeSpan.FromMinutes(2), processingToken: new ProcessingToken(2));

    Assert.True(result);
    var processing = Assert.Single(station.ProcessingInfo);
    Assert.Equal(0, processing.Key);
    Assert.Equal(second, processing.Value.TrackingSubjectId);
  }

  private static StationRuntimeState CreateStation(int workerCapacity)
  {
    var stageId = StageId.NewId();
    return new StationRuntimeState(StationId.NewId(), stageId, workerCapacity);
  }
}
