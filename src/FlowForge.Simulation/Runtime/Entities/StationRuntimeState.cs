using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class StationRuntimeState(
  StationId StationId,
  StageId StageId,
  int WorkerCapacity
  )
{
  public StationId StationId { get; } = StationId;
  public StageId StageId { get; } = StageId;
  public int WorkerCapacity { get; } = WorkerCapacity;
  public int BusyWorkerCount => _workerActive.Count(b => b);

  private readonly List<bool> _workerActive = [.. Enumerable.Repeat(false, WorkerCapacity)];

  private readonly Dictionary<int, StationProcessingInfo> _processingInfo = new Dictionary<int, StationProcessingInfo>();
  public IReadOnlyDictionary<int, StationProcessingInfo> ProcessingInfo => _processingInfo;
  public int AvailableWorkerCount => WorkerCapacity - BusyWorkerCount;
  public bool HasFreeWorker => AvailableWorkerCount > 0;

  public bool TryReserveWorker(
    TrackingSubjectId trackingSubjectId,
    TimeSpan startedAt,
    long processingToken)
  {
    if (AvailableWorkerCount <= 0)
    {
      return false;
    }
    for (int i = 0; i < WorkerCapacity; i++)
    {
      if (!_workerActive[i])
      {
        _workerActive[i] = true;
        _processingInfo[i] = new StationProcessingInfo(trackingSubjectId, i, startedAt, processingToken);
        return true;
      }
    }
    return false;
  }

  public void ReleaseWorker(TrackingSubjectId trackingSubjectId)
  {
    try
    {
      var processingInfo = GetProcessingInfo(trackingSubjectId);
      if (processingInfo.WorkerSlot < 0 || processingInfo.WorkerSlot >= WorkerCapacity)
      {
        throw new ArgumentOutOfRangeException($"StationRuntimeState->ReleaseWorker: Worker slot must be between 0 and {WorkerCapacity - 1}.");
      }
      _workerActive[processingInfo.WorkerSlot] = false;
      _processingInfo.Remove(processingInfo.WorkerSlot);
    }
    catch (InvalidOperationException)
    {
      throw new ArgumentException($"StationRuntimeState->ReleaseWorker: No worker found for TrackingSubjectId {trackingSubjectId}.");
    }
  }

  public StationProcessingInfo GetProcessingInfo(TrackingSubjectId trackingSubjectId)
  {
    try
    {
      return _processingInfo.First(kvp => kvp.Value.TrackingSubjectId == trackingSubjectId).Value;
    }
    catch (InvalidOperationException)
    {
      throw new ArgumentException($"StationRuntimeState->GetProcessingInfo: No worker found for TrackingSubjectId {trackingSubjectId}.");
    }
  }
}
