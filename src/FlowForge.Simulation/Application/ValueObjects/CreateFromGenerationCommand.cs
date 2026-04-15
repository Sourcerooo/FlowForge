using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Application.ValueObjects;

public sealed record CreateFromGenerationCommand(
  TrackingSubjectId TrackingSubjectId,
  TimeSpan ArrivalTime,
  SimulationCommandContext SimulationContext);
