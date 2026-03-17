using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Snapshots.Entities;

public sealed record SimulationSnapshot(
  SimulationRunId RunId,
  long SnapshotSequence,
  TimeSpan SimulationTime,
  SimulationState State,
  ScenarioSnapshot Scenario,
  ProcessSnapshot Process,
  KpiSnapshot Kpi,
  SnapshotMetadata Metadata
  );
