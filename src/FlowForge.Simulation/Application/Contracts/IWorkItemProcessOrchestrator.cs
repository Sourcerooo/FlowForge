namespace FlowForge.Simulation.Orchestration.Contracts;

public interface IWorkItemProcessOrchestrator
{
  public void CreateFromGeneration();
  public void QueueForStage();
  public void StartProcessing();
  public void PutOnHold();
  public void CompleteProcessing();
  public void CompleteWorkItem();

}
