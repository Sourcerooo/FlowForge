using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Services;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Entities.Stages;

namespace FlowForge.Simulation.Tests;

public sealed class StageServiceTests
{
  [Fact]
  public void StopAndRequeue_WhenSameWorkItemIsStoppedTwice_UsesRepeatedOccurrenceOnSecondCall()
  {
    var stageId = StageId.NewId();
    var trackingSubjectId = TrackingSubjectId.NewId();
    var runtimeStore = new RecordingStageRuntimeStateStore();
    var trackingStore = new RecordingStageTrackingStore();
    runtimeStore.StopAndRequeueResults.Enqueue(Result<StageEntry>.Success(
      new StageEntry(trackingSubjectId, default, TimeSpan.FromSeconds(1), default, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), stageId)));
    runtimeStore.StopAndRequeueResults.Enqueue(Result<StageEntry>.Success(
      new StageEntry(trackingSubjectId, default, TimeSpan.FromSeconds(8), default, TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(12), stageId)));
    var sut = new StageService(runtimeStore, trackingStore);

    sut.StopAndRequeue(stageId, trackingSubjectId, TimeSpan.FromSeconds(5));
    sut.StopAndRequeue(stageId, trackingSubjectId, TimeSpan.FromSeconds(12));

    Assert.Equal([OnHoldOccurrence.First, OnHoldOccurrence.Repeated], trackingStore.StopAndRequeueOccurrences);
  }

  [Fact]
  public void PutOnHold_WhenSameWorkItemIsPutOnHoldTwice_UsesRepeatedOccurrenceOnSecondCall()
  {
    var stageId = StageId.NewId();
    var trackingSubjectId = TrackingSubjectId.NewId();
    var runtimeStore = new RecordingStageRuntimeStateStore();
    var trackingStore = new RecordingStageTrackingStore();
    runtimeStore.PutOnHoldResults.Enqueue(Result<StageEntry>.Success(
      new StageEntry(trackingSubjectId, default, TimeSpan.FromSeconds(1), default, TimeSpan.FromSeconds(5), default, stageId)));
    runtimeStore.PutOnHoldResults.Enqueue(Result<StageEntry>.Success(
      new StageEntry(trackingSubjectId, default, TimeSpan.FromSeconds(8), default, TimeSpan.FromSeconds(12), default, stageId)));
    var sut = new StageService(runtimeStore, trackingStore);

    sut.PutOnHold(stageId, trackingSubjectId, TimeSpan.FromSeconds(5));
    sut.PutOnHold(stageId, trackingSubjectId, TimeSpan.FromSeconds(12));

    Assert.Equal([OnHoldOccurrence.First, OnHoldOccurrence.Repeated], trackingStore.PutOnHoldOccurrences);
  }

  [Fact]
  public void PutOnHoldAndStopAndRequeue_ShareOnHoldOccurrenceHistory()
  {
    var stageId = StageId.NewId();
    var trackingSubjectId = TrackingSubjectId.NewId();
    var runtimeStore = new RecordingStageRuntimeStateStore();
    var trackingStore = new RecordingStageTrackingStore();
    runtimeStore.PutOnHoldResults.Enqueue(Result<StageEntry>.Success(
      new StageEntry(trackingSubjectId, default, TimeSpan.FromSeconds(1), default, TimeSpan.FromSeconds(5), default, stageId)));
    runtimeStore.StopAndRequeueResults.Enqueue(Result<StageEntry>.Success(
      new StageEntry(trackingSubjectId, default, TimeSpan.FromSeconds(8), default, TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(12), stageId)));
    var sut = new StageService(runtimeStore, trackingStore);

    sut.PutOnHold(stageId, trackingSubjectId, TimeSpan.FromSeconds(5));
    sut.StopAndRequeue(stageId, trackingSubjectId, TimeSpan.FromSeconds(12));

    Assert.Equal([OnHoldOccurrence.First], trackingStore.PutOnHoldOccurrences);
    Assert.Equal([OnHoldOccurrence.Repeated], trackingStore.StopAndRequeueOccurrences);
  }

  [Fact]
  public void StopAndRequeueAndPutOnHold_ShareOnHoldOccurrenceHistory()
  {
    var stageId = StageId.NewId();
    var trackingSubjectId = TrackingSubjectId.NewId();
    var runtimeStore = new RecordingStageRuntimeStateStore();
    var trackingStore = new RecordingStageTrackingStore();
    runtimeStore.StopAndRequeueResults.Enqueue(Result<StageEntry>.Success(
      new StageEntry(trackingSubjectId, default, TimeSpan.FromSeconds(1), default, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), stageId)));
    runtimeStore.PutOnHoldResults.Enqueue(Result<StageEntry>.Success(
      new StageEntry(trackingSubjectId, default, TimeSpan.FromSeconds(8), default, TimeSpan.FromSeconds(12), default, stageId)));
    var sut = new StageService(runtimeStore, trackingStore);

    sut.StopAndRequeue(stageId, trackingSubjectId, TimeSpan.FromSeconds(5));
    sut.PutOnHold(stageId, trackingSubjectId, TimeSpan.FromSeconds(12));

    Assert.Equal([OnHoldOccurrence.First], trackingStore.StopAndRequeueOccurrences);
    Assert.Equal([OnHoldOccurrence.Repeated], trackingStore.PutOnHoldOccurrences);
  }

  private sealed class RecordingStageRuntimeStateStore : IStageRuntimeStateStore
  {
    public Queue<Result<StageEntry>> StopAndRequeueResults { get; } = new();
    public Queue<Result<StageEntry>> PutOnHoldResults { get; } = new();

    public Result<StageEntry> CompleteProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime) => throw new NotImplementedException();
    public StageEntry? Dequeue(StageId stageId) => throw new NotImplementedException();
    public Result<StageEntry> Enqueue(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime) => throw new NotImplementedException();
    public bool IsBusy(StageId stageId) => throw new NotImplementedException();
    public Result<StageEntry> PutOnHold(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime) => PutOnHoldResults.Dequeue();
    public Result<StageEntry> ResumeProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime) => throw new NotImplementedException();
    public Result<StageEntry> StopAndRequeue(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime) => StopAndRequeueResults.Dequeue();
    public Result<StageEntry> TryStartProcessing(StageId stageId, TimeSpan startedAt) => throw new NotImplementedException();
  }

  private sealed class RecordingStageTrackingStore : IStageTrackingStore
  {
    public List<OnHoldOccurrence> PutOnHoldOccurrences { get; } = [];
    public List<OnHoldOccurrence> StopAndRequeueOccurrences { get; } = [];

    public Result<StageTracking> CompleteWorkItem(StageId stageId, StageEntry stageEntry) => Result<StageTracking>.Success(CreateTracking(stageId));
    public Result<StageTracking> EnqueueWorkItem(StageId stageId, StageEntry stageEntry, OnQueueOccurrence onQueueOccurrence) => Result<StageTracking>.Success(CreateTracking(stageId));
    public Result<StageTracking> GetStageTracking(StageId stageId) => Result<StageTracking>.Success(CreateTracking(stageId));
    public Result<StageTracking> PutOnHoldWorkItem(StageId stageId, StageEntry stageEntry, OnHoldOccurrence onHoldOccurrence)
    {
      PutOnHoldOccurrences.Add(onHoldOccurrence);
      return Result<StageTracking>.Success(CreateTracking(stageId));
    }

    public Result<StageTracking> StartProcessingWorkItem(StageId stageId, StageEntry stageEntry, StageTracking.ProcessingKind entryKind) => Result<StageTracking>.Success(CreateTracking(stageId));
    public Result<StageTracking> StopAndRequeueWorkItem(StageId stageId, StageEntry stageEntry, OnHoldOccurrence onHoldOccurrence)
    {
      StopAndRequeueOccurrences.Add(onHoldOccurrence);
      return Result<StageTracking>.Success(CreateTracking(stageId));
    }

    private static StageTracking CreateTracking(StageId stageId)
    {
      var stationId = StationId.NewId();
      return new StageTracking(
        stageId,
        new Dictionary<StationId, FlowForge.Simulation.Tracking.Entities.Stations.StationTracking>
        {
          [stationId] = new(stationId, stageId)
        });
    }
  }
}
