using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Tracking.Entities.WorkItems;

namespace FlowForge.Simulation.Tracking.Contracts;

public interface IWorkItemTrackingStore
{
  public Result<WorkItemTracking> GetWorkItemTracking(
    TrackingSubjectId trackingSubjectId);

  public WorkItemTracking AddWorkItemTracking(
    TrackingSubjectId trackingSubjectId,
    TimeSpan createdAt,
    TimeSpan? completedAt = null);
  public Result EnqueueWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime);
  public Result StartProcessingWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime);
  public Result StopProcessingWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime);
  public Result CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan completionTime);
}
