using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Orchestration.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Scheduling.Contracts;
using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Application.Services;

public sealed class WorkItemProcessOrchestrator(ISimulationEventScheduler EventScheduler) : IWorkItemProcessOrchestrator
{
  public void CompleteProcessing(
    TrackingSubjectId trackingSubject,
    SimulationEvent simulationEvent,
    SimulationExecutionContext context
    )
  {
    var workItemResult = context.Data.WorkItemTrackingStore.GetWorkItemTracking(trackingSubject);
    if (workItemResult.IsFailure
      || workItemResult.Value is null
      || workItemResult.Value.CurrentStage is null)
    {
      throw new KeyNotFoundException("Something went wrong");
    }
    if (workItemResult.Value.CurrentProcessingToken != simulationEvent.ProcessingToken
      || workItemResult.Value.CurrentStatus != Tracking.Enums.WorkItemStatus.Processing
      || workItemResult.Value.CurrentStage != simulationEvent.StageId)
    {
      //Event outdated, skip
      return;
    }
    var stageResult = context.Data.StageTrackingStore.GetStageTracking(workItemResult.Value.CurrentStage.Value);
    if (stageResult.IsFailure
      || stageResult.Value is null
      || workItemResult.Value.CurrentStation is null
      || !stageResult.Value.Stations.TryGetValue(workItemResult.Value.CurrentStation.Value, out var station))
    {
      throw new KeyNotFoundException("Something went wrong");
    }

    workItemResult.Value.CompleteWorkItem(simulationEvent.ScheduledTime);
    //station.Value.CompleteProcessing(simulationEvent.ScheduledTime);//
    //stageResult.Value.CompleteProcessing(simulationEvent.ScheduledTime);


    //Determine next stage
    var nextStage = workItemResult.Value.CurrentStage;
    EventScheduler.Schedule(
      new WorkItemQueueEvent(
          SimulationEventId.NewId(),
          simulationEvent.SimulationRunId,
          simulationEvent.ScheduledTime,
          context.Data.State.GetNextSequenceNumber(),
          nextStage,
          null,
          0,
          null
        )
      );

    //Update KPI
  }
  public void CompleteWorkItem() => throw new NotImplementedException();
  public void CreateFromGeneration() => throw new NotImplementedException();
  public void PutOnHold() => throw new NotImplementedException();
  public void QueueForStage() => throw new NotImplementedException();
  public void StartProcessing() => throw new NotImplementedException();
}
