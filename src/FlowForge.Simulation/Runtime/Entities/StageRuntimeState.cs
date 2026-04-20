using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed partial class StageRuntimeState(
  StageId stageId,
  IReadOnlyDictionary<StationId, StationRuntimeState> stations
  )
{
  public StageId StageId { get; } = stageId;
  public IReadOnlyDictionary<StationId, StationRuntimeState> Stations { get; } = stations;

  public IReadOnlyCollection<StageEntry> Queue => _queue;

  public void Enqueue(StageEntry entry)
  {
    _queue.Enqueue(entry);
  }
  public StageEntry Dequeue()
  {
    return _queue.Count == 0
      ? throw new InvalidOperationException("StageRuntimeState->Dequeue: Queue is empty.")
      : _queue.Dequeue();
  }

  public bool IsBusy()
  {
    return !Stations.Any(station => station.Value.HasFreeWorker);
  }

  public Result<StageEntry> TryStartProcessing(TimeSpan startedAt)
  {
    if (!HasEntries())
    {
      return Result<StageEntry>.Failure(new InvalidOperationException("Queue is empty. No task to start processing."));
    }
    if (IsBusy())
    {
      return Result<StageEntry>.Failure(new InvalidOperationException("All workers are busy. Task could not be started"));
    }
    var entry = Dequeue();
    if (_processingStation.ContainsKey(entry.TrackingSubjectId))
    {
      throw new InvalidOperationException($"StageRuntimeState->TryStartProcessing: TrackingSubjectId {entry.TrackingSubjectId} is already processing at a station.");
    }
    var stationId = TryReserveFreeStation(entry.TrackingSubjectId, startedAt);
    //If no station could be reserved (which can happen if the queue is not empty but all stations became busy since the IsBusy check),
    //re-enqueue the entry and return false to indicate that processing could not be started.
    if (stationId == null)
    {
      Enqueue(entry);
      return Result<StageEntry>.Failure(new InvalidOperationException("All workers are busy. Task could not be started"));
    }
    return Result<StageEntry>.Success(new StageEntry(entry.TrackingSubjectId, entry.EnqueuedAt, startedAt, default, default, StageId, stationId.Value));
  }

  public Result<StageEntry> StopProcessing(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    if (!_processingStation.TryGetValue(trackingSubjectId, out var stationId))
    {
      return Result<StageEntry>.Failure(new InvalidOperationException($"StageRuntimeState->StopProcessing: TrackingSubjectId {trackingSubjectId} is not currently processing at any station."));
    }
    var station = Stations[stationId];
    var processingInfo = ReleaseStation(station, trackingSubjectId);
    var stageEntry = new StageEntry(trackingSubjectId, default, processingInfo.StartedAt, default, currentTime, StageId, stationId);
    Enqueue(stageEntry);
    return Result<StageEntry>.Success(stageEntry);
  }

  public Result<StageEntry> CompleteProcessing(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    if (!_processingStation.TryGetValue(trackingSubjectId, out var stationId))
    {
      return Result<StageEntry>.Failure(new InvalidOperationException($"StageRuntimeState->StopProcessing: TrackingSubjectId {trackingSubjectId} is not currently processing at any station."));
    }
    var station = Stations[stationId];
    var processingInfo = ReleaseStation(station, trackingSubjectId);
    var stageEntry = new StageEntry(trackingSubjectId, default, processingInfo.StartedAt, currentTime, default, StageId, stationId);
    return Result<StageEntry>.Success(stageEntry);
  }
  //---------------------------------- Private Methods ----------------------------------
  private readonly List<StationId> _stationOrder = [.. stations.Keys];
  private int _nextStationIndex = 0;
  private Queue<StageEntry> _queue { get; } = new Queue<StageEntry>();
  private readonly Dictionary<TrackingSubjectId, StationId> _processingStation = new Dictionary<TrackingSubjectId, StationId>();

  private StationId? TryReserveFreeStation(
    TrackingSubjectId trackingSubjectId,
    TimeSpan startedAt)
  {
    if (_stationOrder.Count == 0)
    {
      return null;
    }

    for (int offset = 0; offset < Stations.Count; offset++)
    {
      var index = (_nextStationIndex + offset) % Stations.Count;
      var stationId = _stationOrder[index];
      var station = Stations[stationId];
      if (!station.TryReserveWorker(trackingSubjectId, startedAt))
      {
        continue;
      }
      _nextStationIndex = (index + 1) % Stations.Count;
      _processingStation[trackingSubjectId] = stationId;
      return stationId;
    }
    return null;
  }

  private StationProcessingInfo ReleaseStation(StationRuntimeState station, TrackingSubjectId trackingSubjectId)
  {
    var processingInfo = station.GetProcessingInfo(trackingSubjectId);
    station.ReleaseWorker(trackingSubjectId);
    _processingStation.Remove(trackingSubjectId);
    return processingInfo;
  }

  private bool HasEntries()
  {
    return _queue.Count > 0;
  }

}
