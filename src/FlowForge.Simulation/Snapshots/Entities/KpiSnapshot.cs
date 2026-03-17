namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record KpiSnapshot(
  ThroughputKpiSnapshot Throughput,
  LeadTimeKpiSnapshot LeadTime,
  WipKpiSnapshot WorkInProgress,
  BottleneckKpiSnapshot Bottleneck,
  IReadOnlyList<StageKpiSnapshot> StageMetrics,
  IReadOnlyList<KpiTrendPointSnapshot> TrendPoints
  );
