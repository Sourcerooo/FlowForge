using FlowForge.Simulation.Runtime.Enums;

namespace FlowForge.Simulation.Runtime.ValueObjects;

public record SimulationCommandContext(
    SimulationRunId SimulationRunId,
    SimulationState SimulationState
  );
