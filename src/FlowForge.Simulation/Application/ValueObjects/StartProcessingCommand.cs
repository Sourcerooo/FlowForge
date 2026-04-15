using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Application.ValueObjects;

public sealed record StartProcessingCommand(
    StageId StageId,
    SimulationCommandContext SimulationContext);
