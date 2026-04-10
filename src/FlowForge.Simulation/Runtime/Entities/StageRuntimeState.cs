using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class StageRuntimeState(
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

  public bool TryStartProcessing(TimeSpan startedAt, long processingToken)
  {
    if (IsBusy())
    {
      return false;
    }
    var entry = Dequeue();
    if (_processingStation.ContainsKey(entry.TrackingSubjectId))
    {
      throw new InvalidOperationException($"StageRuntimeState->TryStartProcessing: TrackingSubjectId {entry.TrackingSubjectId} is already processing at a station.");
    }
    var stationId = TryReserveFreeStation(entry.TrackingSubjectId, startedAt, processingToken);
    return stationId != null;
  }

  public bool TryFinishProcessing(TrackingSubjectId trackingSubjectId)
  {
    if (!_processingStation.ContainsKey(trackingSubjectId))
    {
      throw new InvalidOperationException($"StageRuntimeState->TryFinishProcessing: TrackingSubjectId {trackingSubjectId} is not currently processing at any station.");
    }
    ReleaseStation(trackingSubjectId);
    return true;
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

  private void ReleaseStation(TrackingSubjectId trackingSubjectId)
  {
    if (!_processingStation.TryGetValue(trackingSubjectId, out var stationId))
    {
      throw new InvalidOperationException($"StageRuntimeState->ReleaseStation: TrackingSubjectId {trackingSubjectId} is not currently processing at any station.");
    }
    var station = Stations[stationId];
    station.ReleaseWorker(trackingSubjectId);
    _processingStation.Remove(trackingSubjectId);
  }


}
