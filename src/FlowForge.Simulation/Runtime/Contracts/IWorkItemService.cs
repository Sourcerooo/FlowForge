using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface IWorkItemService
{
  public void CompleteProcessing(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
  public void CompleteWorkItem(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
  public void CreateFromGeneration(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, TimeSpan createdAt);
  public WorkItemRuntimeState GetWorkItemRuntimeState(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId);
  public void QueueForStage(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, StageId stageId, TimeSpan currentTime, long processingToken = 0);
  public void StartProcessing(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, StationId stationId, TimeSpan currentTime);
  public void StopProcessing(WorkItemStore workItemStore, TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
}
