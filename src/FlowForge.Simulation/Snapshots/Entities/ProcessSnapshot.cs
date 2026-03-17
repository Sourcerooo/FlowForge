using FlowForge.Domain.ProcessModel.ValueObjects;

namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record ProcessSnapshot(
  int OrdersCreated,
  int OrdersCompleted,
  int OrdersInProcess,
  IReadOnlyList<StageSnapshot> Stations,
  IReadOnlyList<WorkItemSnapshot> Orders,
  StageId? BottleneckStageId
  );
