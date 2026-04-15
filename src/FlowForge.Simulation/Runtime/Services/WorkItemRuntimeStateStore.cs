using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Services;

internal class WorkItemRuntimeStateStore() : IWorkItemRuntimeStateStore
{
  private readonly Dictionary<TrackingSubjectId, WorkItemRuntimeState> _workItemRuntimeStates
    = new Dictionary<TrackingSubjectId, WorkItemRuntimeState>();

  public WorkItemRuntimeState GetWorkItemRuntimeState(TrackingSubjectId trackingSubjectId)
  {
    return !_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState)
      ? throw new InvalidOperationException($"WorkItemRuntimeStateStore->GetWorkItemRuntimeState: TrackingSubjectId {trackingSubjectId} does not exist.")
      : workItemRuntimeState;
  }

  public void CreateFromGeneration(TrackingSubjectId trackingSubjectId, TimeSpan createdAt)
  {
    if (_workItemRuntimeStates.ContainsKey(trackingSubjectId))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->CreateFromGeneration: TrackingSubjectId {trackingSubjectId} already exists.");
    }
    _workItemRuntimeStates[trackingSubjectId] = new WorkItemRuntimeState(trackingSubjectId, createdAt);
  }

  public void QueueForStage(TrackingSubjectId trackingSubjectId, StageId stageId, ProcessingToken processingToken = default)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }
    workItemRuntimeState.QueueForStage(stageId, processingToken);
  }

  public void StartProcessing(TrackingSubjectId trackingSubjectId, StationId stationId)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }
    workItemRuntimeState.StartProcessing(stationId);
  }

  public void CompleteProcessing(TrackingSubjectId trackingSubjectId)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }
    workItemRuntimeState.CompleteProcessing();
  }

  public void StopProcessing(TrackingSubjectId trackingSubjectId)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }
    workItemRuntimeState.StopProcessing();
  }

  public void CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }
    workItemRuntimeState.CompleteWorkItem(currentTime);
  }

  public bool ContainsWorkItemRuntimeState(TrackingSubjectId trackingSubjectId)
  {
    return _workItemRuntimeStates.ContainsKey(trackingSubjectId);
  }
}
