using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Application.ValueObjects;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Scheduling.Contracts;
using FlowForge.Simulation.Tracking.Enums;
using Microsoft.Extensions.Logging;

namespace FlowForge.Simulation.Application.Services;

public sealed class WorkItemProcessOrchestrator(
  ISimulationEventScheduler EventScheduler,
  IRoutingPolicy RoutingPolicy,
  IWorkItemService WorkItemService,
  IStageService StageService,
  ILogger<WorkItemProcessOrchestrator> Logger
  ) : IWorkItemProcessOrchestrator
{
  public Task CompleteProcessingAsync(
    CompleteProcessingCommand command,
    CancellationToken cancellationToken)
  {
    if (cancellationToken.IsCancellationRequested)
    {
      return Task.FromCanceled(cancellationToken);
    }
    var workItem = WorkItemService.GetWorkItemRuntimeState(command.TrackingSubjectId);

    if (IsEventOutdated(workItem, WorkItemStatus.Processing, command.ProcessingToken, command.StageId)
      || workItem.CurrentStageId is null)
    {
      //Event outdated, skip
      return Task.CompletedTask;
    }
    StageService.CompleteProcessing(workItem.CurrentStageId.Value, workItem.TrackingSubjectId, command.SimulationContext.SimulationState.CurrentTime);
    WorkItemService.CompleteProcessing(command.TrackingSubjectId, command.SimulationContext.SimulationState.CurrentTime);
    Logger.LogInformation("Completed processing for WorkItem {TrackingSubjectId} at Stage {StageId}", command.TrackingSubjectId, workItem.CurrentStageId);

    //Start next item in queue for the stage
    EventScheduler.Schedule(
     new ProcessingStartEvent(
       SimulationEventId.NewId(),
       command.SimulationContext.SimulationRunId,
       command.SimulationContext.SimulationState.CurrentTime,
       command.SimulationContext.SimulationState.GetNextSequenceNumber(),
       workItem.CurrentStageId.Value)
   );

    //Determine next stage
    var nextStage = RoutingPolicy.GetNextStage(workItem.CurrentStageId);
    //If next stage is null, it means processing is finished for the work item, otherwise queue for next stage
    if (nextStage is null)
    {
      EventScheduler.Schedule(
        new WorkItemCompleteEvent(
            SimulationEventId.NewId(),
            command.SimulationContext.SimulationRunId,
            command.SimulationContext.SimulationState.CurrentTime,
            command.SimulationContext.SimulationState.GetNextSequenceNumber(),
            workItem.CurrentProcessingToken,
            workItem.TrackingSubjectId
          )
        );
    }
    else
    {
      EventScheduler.Schedule(
        new WorkItemQueueEvent(
            SimulationEventId.NewId(),
            command.SimulationContext.SimulationRunId,
            command.SimulationContext.SimulationState.CurrentTime,
            command.SimulationContext.SimulationState.GetNextSequenceNumber(),
            nextStage.Value,
            null,
            workItem.CurrentProcessingToken,
            workItem.TrackingSubjectId
          )
        );
    }



    return Task.CompletedTask;
    //Update KPI
  }
  public Task CompleteWorkItemAsync(
    CompleteWorkItemCommand command,
    CancellationToken cancellationToken
    )
  {
    if (cancellationToken.IsCancellationRequested)
    {
      return Task.FromCanceled(cancellationToken);
    }

    var workItem = WorkItemService.GetWorkItemRuntimeState(command.TrackingSubjectId);

    if (IsEventOutdated(workItem, WorkItemStatus.Completed, command.ProcessingToken)
      || workItem.CurrentStageId is null)
    {
      //Event outdated, skip
      return Task.CompletedTask;
    }

    WorkItemService.CompleteWorkItem(command.TrackingSubjectId, command.SimulationContext.SimulationState.CurrentTime);
    Logger.LogInformation("Completed WorkItem {TrackingSubjectId}", command.TrackingSubjectId);
    return Task.CompletedTask;
  }
  public Task CreateFromGenerationAsync(
    CreateFromGenerationCommand command,
    CancellationToken cancellationToken)
  {
    if (cancellationToken.IsCancellationRequested)
    {
      return Task.FromCanceled(cancellationToken);
    }
    WorkItemService.CreateFromGeneration(command.TrackingSubjectId, command.ArrivalTime);
    Logger.LogInformation("Created WorkItem {TrackingSubjectId} from generation at time {ArrivalTime}", command.TrackingSubjectId, command.ArrivalTime);
    var nextStage = RoutingPolicy.GetNextStage(null);
    if (nextStage == null)
    {
      throw new InvalidOperationException("Initial stage could not be determined");
    }
    EventScheduler.Schedule(
       new WorkItemQueueEvent(
           SimulationEventId.NewId(),
           command.SimulationContext.SimulationRunId,
           command.ArrivalTime,
           command.SimulationContext.SimulationState.GetNextSequenceNumber(),
           nextStage.Value,
           null,
           ProcessingToken.Initial,
           command.TrackingSubjectId
         )
       );
    return Task.CompletedTask;
  }

  public Task PutOnHoldAsync(
    PutOnHoldCommand command,
    CancellationToken cancellationToken)
  {
    if (cancellationToken.IsCancellationRequested)
    {
      return Task.FromCanceled(cancellationToken);
    }
    var workItem = WorkItemService.GetWorkItemRuntimeState(command.TrackingSubjectId);

    if (IsEventOutdated(workItem, WorkItemStatus.Processing, command.ProcessingToken, command.CurrentStageId)
      || workItem.CurrentStageId is null)
    {
      //Event outdated, skip
      return Task.CompletedTask;
    }

    StageService.PutOnHold(command.CurrentStageId, command.TrackingSubjectId, command.SimulationContext.SimulationState.CurrentTime);
    WorkItemService.PutOnHold(command.TrackingSubjectId, command.SimulationContext.SimulationState.CurrentTime);
    Logger.LogInformation("Put on hold WorkItem {TrackingSubjectId} at Stage {StageId}", command.TrackingSubjectId, command.CurrentStageId);
    return Task.CompletedTask;
  }

  public Task StopAndRequeueAsync(
    StopAndRequeueCommand command,
    CancellationToken cancellationToken)
  {
    if (cancellationToken.IsCancellationRequested)
    {
      return Task.FromCanceled(cancellationToken);
    }
    var workItem = WorkItemService.GetWorkItemRuntimeState(command.TrackingSubjectId);

    if (IsEventOutdated(workItem, WorkItemStatus.Processing, command.ProcessingToken, command.CurrentStageId)
      || workItem.CurrentStageId is null)
    {
      //Event outdated, skip
      return Task.CompletedTask;
    }

    StageService.StopAndRequeue(command.CurrentStageId, command.TrackingSubjectId, command.SimulationContext.SimulationState.CurrentTime);

    WorkItemService.PutOnHold(command.TrackingSubjectId, command.SimulationContext.SimulationState.CurrentTime);
    WorkItemService.QueueForStage(command.TrackingSubjectId, command.CurrentStageId, command.SimulationContext.SimulationState.CurrentTime, command.ProcessingToken);
    Logger.LogInformation("Put on hold WorkItem {TrackingSubjectId} at Stage {StageId}", command.TrackingSubjectId, command.CurrentStageId);
    return Task.CompletedTask;
  }

  public Task CancelProcessingAsync(
    CancelCommand command,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }


  public Task QueueForStageAsync(
    QueueForStageCommand command,
    CancellationToken cancellationToken)
  {
    if (cancellationToken.IsCancellationRequested)
    {
      return Task.FromCanceled(cancellationToken);
    }


    var workItem = WorkItemService.GetWorkItemRuntimeState(command.TrackingSubjectId);

    if (IsEventOutdated(workItem, new[] { WorkItemStatus.Created, WorkItemStatus.Completed }, command.ProcessingToken, workItem.CurrentStageId))
    {
      //Event outdated, skip
      return Task.CompletedTask;
    }


    StageService.Enqueue(command.CurrentStageId, workItem.TrackingSubjectId, command.SimulationContext.SimulationState.CurrentTime);
    WorkItemService.QueueForStage(command.TrackingSubjectId, command.CurrentStageId, command.SimulationContext.SimulationState.CurrentTime);
    Logger.LogInformation("Queued WorkItem {TrackingSubjectId} for Stage {StageId}", command.TrackingSubjectId, command.CurrentStageId);
    EventScheduler.Schedule(
      new ProcessingStartEvent(
        SimulationEventId.NewId(),
        command.SimulationContext.SimulationRunId,
        command.SimulationContext.SimulationState.CurrentTime,
        command.SimulationContext.SimulationState.GetNextSequenceNumber(),
        command.CurrentStageId)
    );
    return Task.CompletedTask;
  }

  public Task StartProcessingAsync(
    StartProcessingCommand command,
    CancellationToken cancellationToken)
  {
    if (cancellationToken.IsCancellationRequested)
    {
      return Task.FromCanceled(cancellationToken);
    }
    var result = StageService.TryStartProcessing(command.StageId, command.SimulationContext.SimulationState.CurrentTime);
    if (result.IsFailure || result.Value.StationId is null)
    {
      return Task.CompletedTask;
    }

    WorkItemService.StartProcessing(result.Value.TrackingSubjectId, result.Value.StationId.Value, command.SimulationContext.SimulationState.CurrentTime);
    var workitem = WorkItemService.GetWorkItemRuntimeState(result.Value.TrackingSubjectId);
    if (workitem == null || workitem.CurrentStationId == null)
    {
      throw new InvalidOperationException("WorkItem or StationId cannot be null at this point");
    }
    Logger.LogInformation("Started processing WorkItem {TrackingSubjectId} at Stage {StageId} on Station {StationId}", result.Value.TrackingSubjectId, command.StageId, result.Value.StationId);

    var newTrackingEvent = new ProcessingCompleteEvent(
         SimulationEventId.NewId(),
         command.SimulationContext.SimulationRunId,
         command.SimulationContext.SimulationState.CurrentTime + TimeSpan.FromMinutes(5),
         command.SimulationContext.SimulationState.GetNextSequenceNumber(),
         command.StageId,
         workitem.CurrentStationId.Value,
         workitem.CurrentProcessingToken,
         workitem.TrackingSubjectId
       );
    EventScheduler.Schedule(newTrackingEvent);
    return Task.CompletedTask;
  }

  private static bool IsEventOutdated(
    WorkItemRuntimeState? workItem,
    IReadOnlyCollection<WorkItemStatus> expectedStatus,
    ProcessingToken? expectedProcessingToken,
    StageId? expectedStageId)
  {
    return workItem is null
      ? throw new InvalidOperationException("Something went wrong")
      : workItem.CurrentStageId != expectedStageId
      || workItem.CurrentProcessingToken != expectedProcessingToken
      || !expectedStatus.Contains(workItem.CurrentStatus);
  }

  private static bool IsEventOutdated(
   WorkItemRuntimeState? workItem,
   WorkItemStatus expectedStatus,
   ProcessingToken? expectedProcessingToken,
   StageId? expectedStageId = default)
  {
    return IsEventOutdated(workItem, new[] { expectedStatus }, expectedProcessingToken, expectedStageId);
  }
}
