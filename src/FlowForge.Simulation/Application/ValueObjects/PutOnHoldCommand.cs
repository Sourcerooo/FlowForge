using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Application.ValueObjects;

public sealed record PutOnHoldCommand(
  TrackingSubjectId TrackingSubjectId,
  StageId CurrentStageId,
  ProcessingToken ProcessingToken,
  SimulationCommandContext SimulationContext
  );
