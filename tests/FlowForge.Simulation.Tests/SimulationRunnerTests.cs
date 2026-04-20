using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.Scenarios.Entities;
using FlowForge.Domain.SharedKernel.Util;
using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Scheduling.Contracts;
using FlowForge.Simulation.Snapshots.Contracts;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.Entities.Stages;
using FlowForge.Simulation.Tracking.Entities.WorkItems;

namespace FlowForge.Simulation.Tests;

public sealed class SimulationRunnerTests
{
  [Fact]
  public async Task RunSimulation_SchedulesInitialGenerateEventAtTimeZero()
  {
    var queue = new RecordingQueue([]);
    var scheduler = new RecordingScheduler();
    var dispatcher = new RecordingDispatcher();
    var runner = new SimulationEngine(queue, scheduler, dispatcher);

    var result = await runner.RunSimulationAsync(CreateContext(TimeSpan.FromHours(1)), CancellationToken.None);

    Assert.Equal(SimulationRunResult.Completed, result);
    var initialEvent = Assert.Single(scheduler.ScheduledEvents);
    var generateEvent = Assert.IsType<SimulationEventsGenerateEvent>(initialEvent);
    Assert.Equal(TimeSpan.Zero, generateEvent.ScheduledTime);
    Assert.Equal(0, generateEvent.SequenceNumber);
  }

  [Fact]
  public async Task RunSimulation_AdvancesTimeAndDispatchesDequeuedEvents()
  {
    var simulationEvent = CreateGenerateEvent(TimeSpan.FromMinutes(20), sequenceNumber: 7);
    var queue = new RecordingQueue([simulationEvent]);
    var scheduler = new RecordingScheduler();
    var dispatcher = new RecordingDispatcher();
    var runner = new SimulationEngine(queue, scheduler, dispatcher);
    var context = CreateContext(TimeSpan.FromHours(1));

    var result = await runner.RunSimulationAsync(context, CancellationToken.None);

    Assert.Equal(SimulationRunResult.Completed, result);
    Assert.Equal(TimeSpan.FromMinutes(20), context.State.CurrentTime);
    var dispatched = Assert.Single(dispatcher.DispatchedEvents);
    Assert.Same(simulationEvent, dispatched.Event);
    Assert.Equal(context.SimulationRunId, dispatched.Context.SimulationRunId);
    Assert.Same(context.State, dispatched.Context.State);
  }

  [Fact]
  public async Task RunSimulation_SkipsNullDequeuedEvents()
  {
    var queue = new RecordingQueue([null]);
    var scheduler = new RecordingScheduler();
    var dispatcher = new RecordingDispatcher();
    var runner = new SimulationEngine(queue, scheduler, dispatcher);

    var result = await runner.RunSimulationAsync(CreateContext(TimeSpan.FromHours(1)), CancellationToken.None);

    Assert.Equal(SimulationRunResult.Completed, result);
    Assert.Empty(dispatcher.DispatchedEvents);
  }

  [Fact]
  public async Task RunSimulation_WhenCancellationAlreadyRequested_ReturnsCancelled()
  {
    var queue = new RecordingQueue([CreateGenerateEvent(TimeSpan.FromMinutes(5), sequenceNumber: 1)]);
    var scheduler = new RecordingScheduler();
    var dispatcher = new RecordingDispatcher();
    var runner = new SimulationEngine(queue, scheduler, dispatcher);
    using var cancellationTokenSource = new CancellationTokenSource();
    cancellationTokenSource.Cancel();

    var result = await runner.RunSimulationAsync(CreateContext(TimeSpan.FromHours(1)), cancellationTokenSource.Token);

    Assert.Equal(SimulationRunResult.Cancelled, result);
    Assert.Empty(dispatcher.DispatchedEvents);
  }

  [Fact]
  public async Task RunSimulation_WhenNextEventExceedsPlannedDuration_ShouldNotDispatchThatEvent()
  {
    var queue = new RecordingQueue([CreateGenerateEvent(TimeSpan.FromHours(2), sequenceNumber: 1)]);
    var scheduler = new RecordingScheduler();
    var dispatcher = new RecordingDispatcher();
    var runner = new SimulationEngine(queue, scheduler, dispatcher);

    var result = await runner.RunSimulationAsync(CreateContext(TimeSpan.FromHours(1)), CancellationToken.None);

    Assert.Equal(SimulationRunResult.Completed, result);
    Assert.Empty(dispatcher.DispatchedEvents);
  }

  private static SimulationExecutionContext CreateContext(TimeSpan plannedDuration)
  {
    return new SimulationExecutionContext(
      SimulationRunId.NewId(),
      new SimulationMetadata(DateTimeOffset.UtcNow, "scenario", "1.0.0", new SimulationRunOptions()),
      new SimulationState(),
      CreateProcessConfiguration(plannedDuration));
  }

  private static ProcessConfiguration CreateProcessConfiguration(TimeSpan plannedDuration)
  {
    var stageId = StageId.NewId();
    return ProcessConfiguration.Create(
      "process",
      "Process",
      DateTime.UtcNow,
      plannedDuration,
      new ArrivalProfileDefinition(TimeSpan.FromMinutes(15), 1, 1),
      [new StageDefinition(stageId, "stage-1", "Stage 1", 1, [new StationDefinition(StationId.NewId(), stageId, "station-1", "Station 1", 1, TimeSpan.FromMinutes(5))])]);
  }

  private static SimulationEventsGenerateEvent CreateGenerateEvent(TimeSpan scheduledTime, long sequenceNumber)
  {
    return new SimulationEventsGenerateEvent(
      SimulationEventId.NewId(),
      SimulationRunId.NewId(),
      scheduledTime,
      sequenceNumber);
  }

  private sealed class RecordingQueue(IEnumerable<SimulationEvent?> events) : ISimulationEventQueue
  {
    private readonly Queue<SimulationEvent?> _events = new(events);

    public SimulationEvent? Peek() => _events.Count > 0 ? _events.Peek() : null;

    public void Queue(SimulationEvent nextEvent)
    {
      _events.Enqueue(nextEvent);
    }

    public bool TryDequeue(out SimulationEvent? nextEvent)
    {
      if (_events.Count == 0)
      {
        nextEvent = null;
        return false;
      }

      nextEvent = _events.Dequeue();
      return true;
    }
  }

  private sealed class RecordingScheduler : ISimulationEventScheduler
  {
    public List<SimulationEvent> ScheduledEvents { get; } = [];

    public void Schedule(SimulationEvent simulationEvent)
    {
      ScheduledEvents.Add(simulationEvent);
    }
  }

  private sealed class RecordingDispatcher : ISimulationEventDispatcher
  {
    public List<(SimulationEvent Event, SimulationExecutionHandlerContext Context, CancellationToken Token)> DispatchedEvents { get; } = [];

    public Task DispatchAsync(SimulationEvent simulationEvent, SimulationExecutionHandlerContext context, CancellationToken cancellationToken)
    {
      DispatchedEvents.Add((simulationEvent, context, cancellationToken));
      return Task.CompletedTask;
    }
  }

  private sealed class StubTrackingSubjectStore : ITrackingSubjectStore;

  private sealed class StubWorkItemRuntimeStateStore : IWorkItemRuntimeStateStore
  {
    public WorkItemRuntimeState CompleteProcessing(TrackingSubjectId trackingSubjectId) => throw new NotImplementedException();
    public WorkItemRuntimeState CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan currentTime) => throw new NotImplementedException();
    public bool ContainsWorkItemRuntimeState(TrackingSubjectId trackingSubjectId) => throw new NotImplementedException();
    public WorkItemRuntimeState CreateFromGeneration(TrackingSubjectId trackingSubjectId, TimeSpan createdAt) => throw new NotImplementedException();
    public WorkItemRuntimeState GetWorkItemRuntimeState(TrackingSubjectId trackingSubjectId) => throw new NotImplementedException();
    public WorkItemRuntimeState QueueForStage(TrackingSubjectId trackingSubjectId, StageId stageId, ProcessingToken processingToken = default) => throw new NotImplementedException();
    public WorkItemRuntimeState StartProcessing(TrackingSubjectId trackingSubjectId, StationId stationId) => throw new NotImplementedException();
    public WorkItemRuntimeState StopProcessing(TrackingSubjectId trackingSubjectId) => throw new NotImplementedException();
  }

  private sealed class StubStageRuntimeStateStore : IStageRuntimeStateStore
  {
    public Result<StageEntry> CompleteProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime) => throw new NotImplementedException();
    public StageEntry? Dequeue(StageId stageId) => throw new NotImplementedException();
    public Result<StageEntry> Enqueue(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime) => throw new NotImplementedException();
    public bool IsBusy(StageId stageId) => throw new NotImplementedException();
    public Result<StageEntry> StopProcessing(StageId stageId, TrackingSubjectId trackingSubjectId, TimeSpan currentTime) => throw new NotImplementedException();
    public Result<StageEntry> TryStartProcessing(StageId stageId, TimeSpan startedAt) => throw new NotImplementedException();
  }

  private sealed class StubWorkItemTrackingStore : IWorkItemTrackingStore
  {
    public WorkItemTracking AddWorkItemTracking(TrackingSubjectId trackingSubjectId, TimeSpan createdAt, TimeSpan? completedAt = null) => throw new NotImplementedException();
    public Result CompleteWorkItem(TrackingSubjectId trackingSubjectId, TimeSpan completionTime) => throw new NotImplementedException();
    public Result EnqueueWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime) => throw new NotImplementedException();
    public Result<WorkItemTracking> GetWorkItemTracking(TrackingSubjectId trackingSubjectId) => throw new NotImplementedException();
    public Result StartProcessingWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime) => throw new NotImplementedException();
    public Result StopProcessingWorkItem(WorkItemRuntimeState workItem, TimeSpan currentTime) => throw new NotImplementedException();
  }

  private sealed class StubStageTrackingStore : IStageTrackingStore
  {
    public Result<StageTracking> GetStageTracking(StageId stageId) => throw new NotImplementedException();
  }

  private sealed class StubSnapshotStore : ISnapshotStore;

  private sealed class StubSnapshotTimelineStore : ISnapshotTimelineStore;
}
