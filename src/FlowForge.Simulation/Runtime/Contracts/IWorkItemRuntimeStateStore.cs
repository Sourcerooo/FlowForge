using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface IWorkItemRuntimeStateStore
{
  public void CompleteProcessing(TrackingSubjectId trackingSubjectId);
  public void CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
  public void CreateFromGeneration(TrackingSubjectId trackingSubjectId, TimeSpan createdAt);
  public WorkItemRuntimeState GetWorkItemRuntimeState(TrackingSubjectId trackingSubjectId);
  public bool ContainsWorkItemRuntimeState(TrackingSubjectId trackingSubjectId);
  public void QueueForStage(TrackingSubjectId trackingSubjectId, StageId stageId, ProcessingToken processingToken = default);
  public void StartProcessing(TrackingSubjectId trackingSubjectId, StationId stationId);
  public void StopProcessing(TrackingSubjectId trackingSubjectId);
}
