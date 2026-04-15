using FlowForge.Simulation.Application.Contracts;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.Enums;

namespace FlowForge.Simulation.Runtime.ValueObjects;

public record SimulationCommandContext(
    SimulationRunId SimulationRunId,
    SimulationState SimulationState,
    StageStore StageStore,
    WorkItemStore WorkItemStore,
    IRoutingPolicy RoutingPolicy
  );
