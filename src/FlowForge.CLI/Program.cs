using FlowForge.Application;
using FlowForge.Application.Checkpoints.Contracts;
using FlowForge.Infrastructure;
using FlowForge.Simulation;
using FlowForge.Simulation.Tracking.Contracts;
using FlowForge.Simulation.Tracking.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.CLI;

public class Program
{
  public static async Task Main(string[] args)
  {
    var services = new ServiceCollection();

    services
        .AddApplication()
        .AddInfrastructure()
        .AddSimulation();

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
    Console.WriteLine("--------------------------------------------------");

    var workItemTrackingService = serviceProvider.GetRequiredService<IWorkItemTrackingStore>();
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
    Console.WriteLine("FlowForge CLI is ready.");

    var trackingResult = workItemTrackingService.GetWorkItemTracking(trackingSubjectId);
    if (trackingResult.IsSuccess && trackingResult.Value is not null)
    {
      foreach (var segment in trackingResult.Value.Segments)
      {
        Console.WriteLine($"Segment: {segment.SegmentId}, StageId: {segment.StageId}, StartedAt: {segment.StartedAt}, EndedAt: {segment.EndedAt}, Duration: {segment.Duration}");
      }
    }

    if (args.Contains("wait", StringComparer.OrdinalIgnoreCase))
    {
      Console.WriteLine("Waiting until the container is stopped...");
      await Task.Delay(Timeout.InfiniteTimeSpan);
    }
  }

  private static string ResolveCheckpointPath(string[] args)
      => args.Length > 0 && !string.Equals(args[0], "wait", StringComparison.OrdinalIgnoreCase)
          ? Path.GetFullPath(args[0])
          : Path.GetFullPath(Path.Combine("data", "test_data", "initial-checkpoint.flowforge-run.json"));
}
