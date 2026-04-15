using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Services;

internal class WorkItemService() : IWorkItemService
{
  public WorkItemRuntimeState GetWorkItemRuntimeState(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId)
  {
    return workItemStore.WorkItemRuntimeStore.GetWorkItemRuntimeState(trackingSubjectId);
  }

  public void CreateFromGeneration(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, TimeSpan createdAt)
  {
    if (workItemStore.WorkItemRuntimeStore.ContainsWorkItemRuntimeState(trackingSubjectId))
    {
      throw new InvalidOperationException($"WorkItemRuntimeStateStore->CreateFromGeneration: TrackingSubjectId {trackingSubjectId} already exists.");
    }
    workItemStore.WorkItemRuntimeStore.CreateFromGeneration(trackingSubjectId, createdAt);
    workItemStore.WorkItemTrackingStore.AddWorkItemTracking(trackingSubjectId, createdAt);
  }

  public void QueueForStage(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, StageId stageId, TimeSpan currentTime, ProcessingToken processingToken = default)
  {
    workItemStore.WorkItemRuntimeStore.QueueForStage(trackingSubjectId, stageId, processingToken);
    workItemStore.WorkItemTrackingStore.EnqueueWorkItem(trackingSubjectId, currentTime);
  }

  public void StartProcessing(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, StationId stationId, TimeSpan currentTime)
  {
    workItemStore.WorkItemRuntimeStore.StartProcessing(trackingSubjectId, stationId);
    workItemStore.WorkItemTrackingStore.StartProcessingWorkItem(trackingSubjectId, currentTime);
  }

  public void CompleteProcessing(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    workItemStore.WorkItemRuntimeStore.CompleteProcessing(trackingSubjectId);
    workItemStore.WorkItemTrackingStore.CompleteWorkItem(trackingSubjectId, currentTime);
  }

  public void StopProcessing(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    workItemStore.WorkItemRuntimeStore.StopProcessing(trackingSubjectId);
    workItemStore.WorkItemTrackingStore.StopProcessingWorkItem(trackingSubjectId, currentTime);
  }

  public void CompleteWorkItem(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, TimeSpan currentTime)
  {
    workItemStore.WorkItemRuntimeStore.CompleteWorkItem(trackingSubjectId, currentTime);
    workItemStore.WorkItemTrackingStore.CompleteWorkItem(trackingSubjectId, currentTime);
  }
}
