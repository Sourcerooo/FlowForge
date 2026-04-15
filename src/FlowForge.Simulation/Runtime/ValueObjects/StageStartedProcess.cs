using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed partial class StageRuntimeState
{
  public record struct StageStartedProcess(TrackingSubjectId TrackingSubjectId, StageId StageId, StationId StationId, long ProcessingToken);

}
