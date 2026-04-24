using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Contracts;

public interface IWorkItemService
{
  public void CompleteProcessing(TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
  public void CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
  public void CreateFromGeneration(TrackingSubjectId trackingSubjectId, TimeSpan createdAt);
  public WorkItemRuntimeState GetWorkItemRuntimeState(TrackingSubjectId trackingSubjectId);
  public void QueueForStage(TrackingSubjectId trackingSubjectId, StageId stageId, TimeSpan currentTime, ProcessingToken processingToken = default);
  public void StartProcessing(TrackingSubjectId trackingSubjectId, StationId stationId, TimeSpan currentTime);
  public void PutOnHold(TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
  public void ResumeProcessing(TrackingSubjectId trackingSubjectId, TimeSpan currentTime);
}
