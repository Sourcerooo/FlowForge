using FlowForge.Domain.ProcessModel.ValueObjects;

namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record StageSnapshot(
  StageId StageId,
  string DisplayName,
  int QueueLength,
  int WorkerCount,
  int BusyWorkers,
  int IdleWorkers,
  int OrdersProcessed,
  double Utilization,
  TimeSpan AverageQueueWaitingTime,
  TimeSpan AverageProcessingTime,
  bool IsBottleneck
  );
