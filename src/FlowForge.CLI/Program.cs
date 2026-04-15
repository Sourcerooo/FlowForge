#pragma warning disable CA1848

using System.Globalization;
using FlowForge.Application;
using FlowForge.Application.Checkpoints.Contracts;
using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Infrastructure;
using FlowForge.Simulation;
using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace FlowForge.CLI;

public record struct MyObject(string Name, int Value);

public class Program
{
  public static async Task Main(string[] args)
  {
    var services = new ServiceCollection();
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console(
              outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}",
              restrictedToMinimumLevel: LogEventLevel.Information,
              formatProvider: CultureInfo.InvariantCulture)
        .WriteTo.File("logs/flowforge-cli-.log",
              rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}",
              formatProvider: CultureInfo.InvariantCulture)
        .CreateLogger();

    services.AddLogging(configure =>
    {
      configure.ClearProviders();
      configure.AddSerilog(Log.Logger, dispose: true);
    });

    services
        .AddSimulation()
        .AddApplication()
        .AddInfrastructure();


    var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Building service provider...");

    await using var serviceProvider = services.BuildServiceProvider();
    var checkpointStore = serviceProvider.GetRequiredService<ISimulationCheckpointStore>();


    /*var checkpointPath = ResolveCheckpointPath(args);
    var state = await checkpointStore.LoadAsync(checkpointPath);

    Console.WriteLine("Loaded checkpoint successfully.");
    Console.WriteLine($"Path: {checkpointPath}");
    Console.WriteLine($"RunId: {state.SimulationRunId}");
    Console.WriteLine($"Scenario: {state.ProcessConfiguration.Name} ({state.ProcessConfiguration.ScenarioKey})");
    Console.WriteLine($"Status: {state.RuntimeState.Status}");
    Console.WriteLine($"CurrentTime: {state.RuntimeState.CurrentTime}");
    Console.WriteLine($"Stages: {state.ProcessConfiguration.Stages.Count}");
    Console.WriteLine($"QueueEvents: {state.EventQueue.Count}");
    Console.WriteLine($"LatestSnapshotPresent: {state.SnapshotState.LatestSnapshot is not null}");

    foreach (var stage in state.ProcessConfiguration.Stages)
    {
      Console.WriteLine($"Stage: {stage.DisplayName} ({stage.StageKey}), Stations: {stage.Stations.Count}");

      foreach (var station in stage.Stations)
      {
        Console.WriteLine($"  Station: {station.DisplayName} ({station.StationKey}), Workers: {station.WorkerCount}");
      }
    }

    foreach (var simulationEvent in state.EventQueue)
    {
      Console.WriteLine(
          $"QueuedEvent: {simulationEvent.EventType}, Kind: {simulationEvent.EventKind}, Time: {simulationEvent.ScheduledTime}, Sequence: {simulationEvent.SequenceNumber}");
    }
    */
    logger.LogInformation("FlowForge CLI is ready.");
    logger.LogInformation("--------------------------------------------------");

    var t1 = TrackingSubjectId.NewId();
    var t2 = TrackingSubjectId.NewId();
    var t3 = TrackingSubjectId.NewId();
    var t4 = TrackingSubjectId.NewId();
    var t5 = TrackingSubjectId.NewId();

    var stationRuntimeState = new StationRuntimeState(StationId.NewId(), StageId.NewId(), 3);
    var stageRuntimeState = new StageRuntimeState(
      stationRuntimeState.StageId,
      new Dictionary<StationId, StationRuntimeState> {
        { stationRuntimeState.StationId, stationRuntimeState }
      });

    stageRuntimeState.Enqueue(new StageQueueEntry(t1, TimeSpan.FromSeconds(10)));
    stageRuntimeState.Enqueue(new StageQueueEntry(t2, TimeSpan.FromSeconds(20)));
    stageRuntimeState.Enqueue(new StageQueueEntry(t3, TimeSpan.FromSeconds(30)));
    stageRuntimeState.Enqueue(new StageQueueEntry(t4, TimeSpan.FromSeconds(40)));

    var succ = false;
    succ = stageRuntimeState.TryStartProcessing(TimeSpan.FromSeconds(10)).IsSuccess;
    succ = stageRuntimeState.TryStartProcessing(TimeSpan.FromSeconds(20)).IsSuccess;
    succ = stageRuntimeState.TryStartProcessing(TimeSpan.FromSeconds(30)).IsSuccess;
    succ = stageRuntimeState.TryStartProcessing(TimeSpan.FromSeconds(40)).IsSuccess;
    stageRuntimeState.CompleteProcessing(t2);
    stageRuntimeState.Enqueue(new StageQueueEntry(t5, TimeSpan.FromSeconds(50)));
    succ = stageRuntimeState.TryStartProcessing(TimeSpan.FromSeconds(50)).IsSuccess;

    /*var workItemTrackingService = serviceProvider.GetRequiredService<IWorkItemTrackingStore>();
    var trackingSubjectId = TrackingSubjectId.NewId();

    // Generator creates order
    workItemTrackingService.AddWorkItemTracking(trackingSubjectId, TimeSpan.FromSeconds(10));
    // Simulator enqueues order for picking
    workItemTrackingService.EnqueueWorkItem(trackingSubjectId, TimeSpan.FromSeconds(10));
    // Simulator processes order for picking
    workItemTrackingService.ProcessWorkItem(trackingSubjectId, TimeSpan.FromSeconds(50));
    // Simulator stops processing order for picking (on Hold)
    workItemTrackingService.StopWorkItem(trackingSubjectId, TimeSpan.FromSeconds(60));
    // Simulator processes order for picking again
    workItemTrackingService.ProcessWorkItem(trackingSubjectId, TimeSpan.FromSeconds(120));
    // Simulator stops processing order for picking again (order completed)
    workItemTrackingService.CompleteWorkItem(trackingSubjectId, TimeSpan.FromSeconds(150));
    //orderTracking.Segments.


    var trackingResult = workItemTrackingService.GetWorkItemTracking(trackingSubjectId);
    if (trackingResult.IsSuccess && trackingResult.Value is not null)
    {
      foreach (var segment in trackingResult.Value.Segments)
      {
        if (logger.IsEnabled(LogLevel.Information))
        {
          logger.LogInformation("Segment: {SegmentId}, StageId: {StageId}, StartedAt: {StartedAt}, EndedAt: {EndedAt}, Duration: {Duration}",
             segment.SegmentId,
             segment.StageId,
             segment.StartedAt,
             segment.EndedAt,
             segment.Duration
          );
        }
      }
    }
    */


    var simulationRunner = serviceProvider.GetRequiredService<ISimulationRunner>();

    await simulationRunner.RunAsync(CancellationToken.None);


    if (args.Contains("wait", StringComparer.OrdinalIgnoreCase))
    {
      logger.LogInformation("Waiting until the container is stopped...");
      await Task.Delay(Timeout.InfiniteTimeSpan);
    }
  }


  private static string ResolveCheckpointPath(string[] args)
      => args.Length > 0 && !string.Equals(args[0], "wait", StringComparison.OrdinalIgnoreCase)
          ? Path.GetFullPath(args[0])
          : Path.GetFullPath(Path.Combine("data", "test_data", "initial-checkpoint.flowforge-run.json"));
}

#pragma warning restore CA1848
