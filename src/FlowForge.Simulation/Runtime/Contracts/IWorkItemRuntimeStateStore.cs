using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface IWorkItemRuntimeStateStore
{
  public WorkItemRuntimeState CompleteProcessing(TrackingSubjectId trackingSubjectId);
  public WorkItemRuntimeState CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
  public WorkItemRuntimeState CreateFromGeneration(TrackingSubjectId trackingSubjectId, TimeSpan createdAt);
  public WorkItemRuntimeState GetWorkItemRuntimeState(TrackingSubjectId trackingSubjectId);
  public bool ContainsWorkItemRuntimeState(TrackingSubjectId trackingSubjectId);
  public WorkItemRuntimeState QueueForStage(TrackingSubjectId trackingSubjectId, StageId stageId, ProcessingToken processingToken = default);
  public WorkItemRuntimeState StartProcessing(TrackingSubjectId trackingSubjectId, StationId stationId);
  public WorkItemRuntimeState StopProcessing(TrackingSubjectId trackingSubjectId);
}
