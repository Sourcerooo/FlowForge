using FlowForge.Simulation.Events.SimulationEvents;
using FlowForge.Simulation.Runtime.Entities;
using FlowForge.Simulation.Tracking.ValueObjects;

namespace FlowForge.Simulation.Application.Contracts;

public interface IWorkItemProcessOrchestrator
{
  public void CreateFromGeneration();
  public void QueueForStage();
  public void StartProcessing();
  public void PutOnHold();
  public void CompleteProcessing(TrackingSubjectId trackingSubject,
    SimulationEvent simulationEvent,
    SimulationExecutionContext context);
  public void CompleteWorkItem();

}
