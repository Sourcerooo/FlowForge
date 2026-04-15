using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.Scenarios.Entities;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.Services;
using FlowForge.Simulation.Runtime.ValueObjects;
using FlowForge.Simulation.Tracking.Entities.Stages;
using FlowForge.Simulation.Tracking.Services;
using Microsoft.Extensions.Logging;

namespace FlowForge.Simulation.Runtime.Entities;

public class SimulationContextBuilder(
  ILoggerFactory LoggerFactory
 /*ITrackingSubjectStore TrackingSubjectStore,*/
 //IWorkItemTrackingStore WorkItemTrackingStore
 /*IStageTrackingStore StageTrackingStore,
 ISnapshotStore SnapshotStore,
 ISnapshotTimelineStore SnapshotTimelineStore*/) : ISimulationContextBuilder
{
  public SimulationExecutionContext Build()
  {
    var stage1Id = StageId.NewId();
    var stage2Id = StageId.NewId();
    var processConfiguration = ProcessConfiguration.Create(
          "test-process", "Test Process", DateTime.UtcNow, TimeSpan.FromHours(2),
          new ArrivalProfileDefinition(TimeSpan.FromMinutes(60), 10, 10),
          new List<StageDefinition>
          {
            new StageDefinition(stage1Id, "stage-1", "Stage 1", 1, new List<StationDefinition>{
              new StationDefinition(StationId.NewId(), stage1Id, "station-1-1", "Station 1-1", 3, TimeSpan.FromMinutes(5))
            }),
            new StageDefinition(stage2Id, "stage-2", "Stage 2", 2, new List<StationDefinition>{
              new StationDefinition(StationId.NewId(), stage2Id, "station-2-1", "Station 2-1", 3, TimeSpan.FromMinutes(5))
            }),
          });
    var workItemTrackingStore = new WorkItemTrackingStore(LoggerFactory.CreateLogger<WorkItemTrackingStore>());
    var workItemRuntimeStore = new WorkItemRuntimeStateStore();
    var stageTrackingStore = new StageTrackingStore();
    var stageRuntimeStateStore = new StageRuntimeStateStore(processConfiguration.Stages);

    var routingPolicy = new RoutingPolicy();


    return new SimulationExecutionContext(
      SimulationRunId.NewId(),
      new SimulationExecutionContextData
      {
        ProcessConfiguration = processConfiguration,
        Metadata = new SimulationMetadata(DateTime.UtcNow, "test-scenario", "1.0.0", new SimulationRunOptions()),
        State = new SimulationState(),
        //TrackingSubjectStore = TrackingSubjectStore,
        WorkItemTrackingStore = workItemTrackingStore,
        WorkItemRuntimeStateStore = workItemRuntimeStore,
        StageTrackingStore = stageTrackingStore,
        StageRuntimeStateStore = stageRuntimeStateStore,
        RoutingPolicy = routingPolicy
        //SnapshotStore = SnapshotStore,
        //SnapshotTimelineStore = SnapshotTimelineStore
      });

  }
}
