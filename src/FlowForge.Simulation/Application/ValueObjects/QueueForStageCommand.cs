using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Application.ValueObjects;

public sealed record QueueForStageCommand(
    TrackingSubjectId TrackingSubjectId,
    ProcessingToken? ProcessingToken,
    StageId CurrentStageId,
    SimulationCommandContext SimulationContext
  );
