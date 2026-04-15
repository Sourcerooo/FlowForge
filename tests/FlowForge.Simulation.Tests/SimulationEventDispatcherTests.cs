using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.Scenarios.Entities;
using FlowForge.Simulation.Events.Contracts;
using FlowForge.Simulation.Events.Entities;
using FlowForge.Simulation.Events.Enums;
using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Events.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Tests;

public sealed class SimulationEventDispatcherTests
{
  [Fact]
  public async Task DispatchAsync_CallsMatchingHandlerExactlyOnce()
  {
    var matchingHandler = new RecordingHandler(EventKind.WorkItemQueue);
    var otherHandler = new RecordingHandler(EventKind.ProcessingComplete);
    var dispatcher = new SimulationEventDispatcher([matchingHandler, otherHandler]);
    var context = CreateHandlerContext();
    var simulationEvent = CreateQueueEvent();

    await dispatcher.DispatchAsync(simulationEvent, context, CancellationToken.None);

    Assert.Equal(1, matchingHandler.CallCount);
    Assert.Equal(0, otherHandler.CallCount);
    Assert.Same(simulationEvent, matchingHandler.LastEvent);
    Assert.Same(context, matchingHandler.LastContext);
  }

  [Fact]
  public async Task DispatchAsync_ForwardsCancellationTokenToHandler()
  {
    var handler = new RecordingHandler(EventKind.WorkItemQueue);
    var dispatcher = new SimulationEventDispatcher([handler]);
    using var cancellationTokenSource = new CancellationTokenSource();
    cancellationTokenSource.Cancel();

    await dispatcher.DispatchAsync(CreateQueueEvent(), CreateHandlerContext(), cancellationTokenSource.Token);

    Assert.True(handler.LastCancellationToken.IsCancellationRequested);
  }

  [Fact]
  public async Task DispatchAsync_WhenNoHandlerExists_ShouldThrowInvalidOperationException()
  {
    var dispatcher = new SimulationEventDispatcher([new RecordingHandler(EventKind.ProcessingComplete)]);

    Task Act() => dispatcher.DispatchAsync(CreateQueueEvent(), CreateHandlerContext(), CancellationToken.None);

    await Assert.ThrowsAsync<InvalidOperationException>(Act);
  }

  private static SimulationExecutionHandlerContext CreateHandlerContext()
  {
    return new SimulationExecutionHandlerContext
    {
      SimulationRunId = SimulationRunId.NewId(),
      ProcessConfiguration = CreateProcessConfiguration(TimeSpan.FromHours(1)),
      Metadata = new SimulationMetadata(DateTimeOffset.UtcNow, "scenario", "1.0.0", new SimulationRunOptions()),
      State = new SimulationState()
    };
  }

  private static WorkItemQueueEvent CreateQueueEvent()
  {
    return new WorkItemQueueEvent(
      SimulationEventId.NewId(),
      SimulationRunId.NewId(),
      TimeSpan.FromMinutes(5),
      3,
      StageId.NewId(),
      StationId.NewId(),
      4,
      TrackingSubjectId.NewId());
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

  private sealed class RecordingHandler(EventKind canHandle) : ISimulationEventHandler
  {
    public int CallCount { get; private set; }
    public SimulationEvent? LastEvent { get; private set; }
    public SimulationExecutionHandlerContext? LastContext { get; private set; }
    public CancellationToken LastCancellationToken { get; private set; }

    public EventKind CanHandle() => canHandle;

    public Task Process(SimulationEvent simulationEvent, SimulationExecutionHandlerContext context, CancellationToken cancellationToken)
    {
      CallCount++;
      LastEvent = simulationEvent;
      LastContext = context;
      LastCancellationToken = cancellationToken;
      return Task.CompletedTask;
    }
  }
}
