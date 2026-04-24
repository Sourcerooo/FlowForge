using global::FlowForge.Domain.Orders.ValueObjects;
using global::FlowForge.Domain.Process.ValueObjects;
using global::FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Application.ValueObjects;

public sealed record StopAndRequeueCommand(
  TrackingSubjectId TrackingSubjectId,
  StageId CurrentStageId,
  ProcessingToken ProcessingToken,
  SimulationCommandContext SimulationContext
  );

