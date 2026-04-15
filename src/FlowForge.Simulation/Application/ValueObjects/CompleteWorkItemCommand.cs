using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Application.ValueObjects;

public sealed record CompleteWorkItemCommand(
  TrackingSubjectId TrackingSubjectId,
  ProcessingToken ProcessingToken,
  SimulationCommandContext SimulationContext
  );
