using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Tracking.Contracts;

namespace FlowForge.Simulation.Runtime.Entities;

public sealed class WorkItemStore(
  IWorkItemRuntimeStateStore workItemRuntimeStore,
  IWorkItemTrackingStore workItemTrackingStore)
{
  public IWorkItemRuntimeStateStore WorkItemRuntimeStore { get; } = workItemRuntimeStore;
  public IWorkItemTrackingStore WorkItemTrackingStore { get; } = workItemTrackingStore;
};

