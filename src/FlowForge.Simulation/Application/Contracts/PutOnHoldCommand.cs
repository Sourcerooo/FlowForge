using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Simulation.Runtime.Entities;

namespace FlowForge.Simulation.Application.Contracts;

public sealed record PutOnHoldCommand(
  TrackingSubjectId TrackingSubjectId,
  StageId CurrentStageId,
  long ProcessingToken,
  TimeSpan CurrentTime,
  StageStore StageStore,
  WorkItemStore WorkItemStore
  );
