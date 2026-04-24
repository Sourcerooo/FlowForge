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

  public WorkItemRuntimeState CreateFromGeneration(TrackingSubjectId trackingSubjectId, TimeSpan createdAt)
  {
    if (_workItemRuntimeStates.ContainsKey(trackingSubjectId))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->CreateFromGeneration: TrackingSubjectId {trackingSubjectId} already exists.");
    }
    var workItem = new WorkItemRuntimeState(trackingSubjectId, createdAt);
    _workItemRuntimeStates[trackingSubjectId] = workItem;
    return workItem;
  }

  public WorkItemRuntimeState QueueForStage(TrackingSubjectId trackingSubjectId, StageId stageId, ProcessingToken processingToken = default)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }
    workItemRuntimeState.QueueForStage(stageId, processingToken);
    return workItemRuntimeState;
  }

  public WorkItemRuntimeState StartProcessing(TrackingSubjectId trackingSubjectId, StationId stationId)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }
    workItemRuntimeState.StartProcessing(stationId);
    return workItemRuntimeState;
  }

  public WorkItemRuntimeState CompleteProcessing(TrackingSubjectId trackingSubjectId)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }
    workItemRuntimeState.CompleteProcessing();
    return workItemRuntimeState;
  }

  public WorkItemRuntimeState PutOnHold(TrackingSubjectId trackingSubjectId)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }
    workItemRuntimeState.PutOnHold();
    return workItemRuntimeState;
  }

  public WorkItemRuntimeState ResumeProcessing(TrackingSubjectId trackingSubjectId)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }

    workItemRuntimeState.ResumeProcessing();
    return workItemRuntimeState;
  }

  public WorkItemRuntimeState CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    if (!_workItemRuntimeStates.TryGetValue(trackingSubjectId, out var workItemRuntimeState))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->QueueForStage: TrackingSubjectId {trackingSubjectId} does not exist.");
    }
    workItemRuntimeState.CompleteWorkItem(currentTime);
    return workItemRuntimeState;
  }

  public bool ContainsWorkItemRuntimeState(TrackingSubjectId trackingSubjectId)
  {
    return _workItemRuntimeStates.ContainsKey(trackingSubjectId);
  }
}
