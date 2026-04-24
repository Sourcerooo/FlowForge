using FlowForge.Simulation.Application.ValueObjects;

namespace FlowForge.Simulation.Application.Contracts;

public interface IWorkItemProcessOrchestrator
{
  public Task CreateFromGenerationAsync(
    CreateFromGenerationCommand command,
    CancellationToken cancellationToken);
  public Task QueueForStageAsync(
    QueueForStageCommand command,
    CancellationToken cancellationToken);
  public Task StartProcessingAsync(
    StartProcessingCommand command,
    CancellationToken cancellationToken);
  public Task PutOnHoldAsync(
    PutOnHoldCommand command,
    CancellationToken cancellationToken);

  public Task StopAndRequeueAsync(
    StopAndRequeueCommand command,
    CancellationToken cancellationToken);

  public Task CancelProcessingAsync(
    CancelCommand command,
    CancellationToken cancellationToken);

  public Task CompleteProcessingAsync(
    CompleteProcessingCommand command,
    CancellationToken cancellationToken);
  public Task CompleteWorkItemAsync(
    CompleteWorkItemCommand command,
    CancellationToken cancellationToken);

}
