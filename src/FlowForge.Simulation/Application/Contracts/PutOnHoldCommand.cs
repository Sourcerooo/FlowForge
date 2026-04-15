using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Application.Contracts;

public sealed record PutOnHoldCommand(
  TrackingSubjectId TrackingSubjectId,
  StageId CurrentStageId,
  ProcessingToken ProcessingToken,
  TimeSpan CurrentTime,
  StageStore StageStore,
  WorkItemStore WorkItemStore
  );
