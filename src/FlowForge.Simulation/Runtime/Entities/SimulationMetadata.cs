namespace FlowForge.Simulation.Runtime.Entities;

public sealed record SimulationMetadata(
  DateTimeOffset CreatedAtUtc,
  string ScenarioKey,
  string EngineVersion,
  SimulationRunOptions Options
  );
