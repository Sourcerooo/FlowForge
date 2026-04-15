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

  public IReadOnlyCollection<StageQueueEntry> Queue => _queue;

  public void Enqueue(StageQueueEntry entry)
  {
    _queue.Enqueue(entry);
  }
  public StageQueueEntry Dequeue()
  {
    return _queue.Count == 0
      ? throw new InvalidOperationException("StageRuntimeState->Dequeue: Queue is empty.")
      : _queue.Dequeue();
  }

  public bool IsBusy()
  {
    return !Stations.Any(station => station.Value.HasFreeWorker);
  }

  public Result<StageStartedProcess> TryStartProcessing(TimeSpan startedAt)
  {
    if (!HasEntries())
    {
      return Result<StageStartedProcess>.Failure(new InvalidOperationException("Queue is empty. No task to start processing."));
    }
    if (IsBusy())
    {
      return Result<StageStartedProcess>.Failure(new InvalidOperationException("All workers are busy. Task could not be started"));
    }
    var entry = Dequeue();
    if (_processingStation.ContainsKey(entry.TrackingSubjectId))
    {
      throw new InvalidOperationException($"StageRuntimeState->TryStartProcessing: TrackingSubjectId {entry.TrackingSubjectId} is already processing at a station.");
    }
    var stationId = TryReserveFreeStation(entry.TrackingSubjectId, startedAt, entry.ProcessingToken);
    //If no station could be reserved (which can happen if the queue is not empty but all stations became busy since the IsBusy check),
    //re-enqueue the entry and return false to indicate that processing could not be started.
    if (stationId == null)
    {
      Enqueue(entry);
      return Result<StageStartedProcess>.Failure(new InvalidOperationException("All workers are busy. Task could not be started"));
    }
    return Result<StageStartedProcess>.Success(new StageStartedProcess(entry.TrackingSubjectId, StageId, stationId.Value, entry.ProcessingToken));
  }

  public void StopProcessing(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    if (!_processingStation.ContainsKey(trackingSubjectId))
    {
      throw new InvalidOperationException($"StageRuntimeState->StopProcessing: TrackingSubjectId {trackingSubjectId} is not currently processing at any station.");
    }
    var proccesingInfo = ReleaseStation(trackingSubjectId);
    var newProcessingToken = proccesingInfo.ProcessingToken + 1;
    Enqueue(new StageQueueEntry(trackingSubjectId, currentTime, newProcessingToken));
  }

  public void CompleteProcessing(TrackingSubjectId trackingSubjectId)
  {
    if (!_processingStation.ContainsKey(trackingSubjectId))
    {
      throw new InvalidOperationException($"StageRuntimeState->CompleteProcessing: TrackingSubjectId {trackingSubjectId} is not currently processing at any station.");
    }
    ReleaseStation(trackingSubjectId);
  }
  //---------------------------------- Private Methods ----------------------------------
  private readonly List<StationId> _stationOrder = [.. stations.Keys];
  private int _nextStationIndex = 0;
  private Queue<StageQueueEntry> _queue { get; } = new Queue<StageQueueEntry>();
  private readonly Dictionary<TrackingSubjectId, StationId> _processingStation = new Dictionary<TrackingSubjectId, StationId>();

  private StationId? TryReserveFreeStation(
    TrackingSubjectId trackingSubjectId,
    TimeSpan startedAt,
    long processingToken)
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
      if (!station.TryReserveWorker(trackingSubjectId, startedAt, processingToken))
      {
        continue;
      }
      _nextStationIndex = (index + 1) % Stations.Count;
      _processingStation[trackingSubjectId] = stationId;
      return stationId;
    }
    return null;
  }

  private StationProcessingInfo ReleaseStation(TrackingSubjectId trackingSubjectId)
  {
    if (!_processingStation.TryGetValue(trackingSubjectId, out var stationId))
    {
      throw new InvalidOperationException($"StageRuntimeState->ReleaseStation: TrackingSubjectId {trackingSubjectId} is not currently processing at any station.");
    }
    var station = Stations[stationId];
    var processingToken = station.GetProcessingInfo(trackingSubjectId);
    station.ReleaseWorker(trackingSubjectId);
    _processingStation.Remove(trackingSubjectId);
    return processingToken;
  }

  private bool HasEntries()
  {
    return _queue.Count > 0;
  }

}
