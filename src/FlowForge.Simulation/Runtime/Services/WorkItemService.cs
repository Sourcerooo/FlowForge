using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.Contracts;

namespace FlowForge.Simulation.Runtime.Services;

internal class WorkItemService(IWorkItemRuntimeStateStore WorkItemRuntimeStateStore,
  IWorkItemTrackingStore WorkItemTrackingStore) : IWorkItemService
{
  public WorkItemRuntimeState GetWorkItemRuntimeState(TrackingSubjectId trackingSubjectId)
  {
    return WorkItemRuntimeStateStore.GetWorkItemRuntimeState(trackingSubjectId);
  }

  public void CreateFromGeneration(TrackingSubjectId trackingSubjectId, TimeSpan createdAt)
  {
    if (WorkItemRuntimeStateStore.ContainsWorkItemRuntimeState(trackingSubjectId))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->CreateFromGeneration: TrackingSubjectId {trackingSubjectId} already exists.");
    }
    WorkItemRuntimeStateStore.CreateFromGeneration(trackingSubjectId, createdAt);
    WorkItemTrackingStore.AddWorkItemTracking(trackingSubjectId, createdAt);
  }

  public void QueueForStage(TrackingSubjectId trackingSubjectId, StageId stageId, TimeSpan currentTime, ProcessingToken processingToken = default)
  {
    var workItem = WorkItemRuntimeStateStore.QueueForStage(trackingSubjectId, stageId, processingToken);
    WorkItemTrackingStore.EnqueueWorkItem(workItem, currentTime);
  }

  public void StartProcessing(TrackingSubjectId trackingSubjectId, StationId stationId, TimeSpan currentTime)
  {
    var workItem = WorkItemRuntimeStateStore.StartProcessing(trackingSubjectId, stationId);
    WorkItemTrackingStore.StartProcessingWorkItem(workItem, currentTime);
  }

  public void CompleteProcessing(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var workItem = WorkItemRuntimeStateStore.CompleteProcessing(trackingSubjectId);
    WorkItemTrackingStore.CompleteProcessingWorkItem(workItem, currentTime);
  }

  public void PutOnHold(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var workItem = WorkItemRuntimeStateStore.PutOnHold(trackingSubjectId);
    WorkItemTrackingStore.StopProcessingWorkItem(workItem, currentTime);
  }

  public void ResumeProcessing(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    var workItem = WorkItemRuntimeStateStore.ResumeProcessing(trackingSubjectId);
    WorkItemTrackingStore.StartProcessingWorkItem(workItem, currentTime);
  }

  public void CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    WorkItemRuntimeStateStore.CompleteWorkItem(trackingSubjectId, currentTime);
    WorkItemTrackingStore.CompleteWorkItem(trackingSubjectId, currentTime);
  }
}
