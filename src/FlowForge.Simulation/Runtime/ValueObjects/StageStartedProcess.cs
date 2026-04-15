using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed partial class StageRuntimeState
{
  public record struct StageStartedProcess(TrackingSubjectId TrackingSubjectId, StageId StageId, StationId StationId, ProcessingToken ProcessingToken);

}
