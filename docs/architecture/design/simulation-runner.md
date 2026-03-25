# Simulation Runner Design

This document is normative for the dequeue-and-dispatch loop, queue ownership, and runner lifecycle.

## Execution Model

```text
StartSimulation use case
  -> load ProcessConfiguration
  -> call ISimulationRunFactory.Create(processConfiguration, options)
  -> factory builds SimulationExecutionContext
  -> factory enqueues first GenerateSimulationEvent through ISimulationScheduler
  -> application calls ISimulationRunner.RunAsync(executionContext, cancellationToken)
```

Recommended rule:

- `RunAsync` receives a fully initialized context
- construction and bootstrap stay separate from execution
- the queue is run-scoped and owned operationally by `SimulationRunner`

## Queue Ownership

Recommended ownership model:

- one simulation run owns exactly one mutable priority queue instance
- `SimulationRunner` owns dequeue access and simulation-time advancement
- `ISimulationScheduler` is the only write gateway into the queue
- handlers and external consumers must never mutate the raw queue directly

Recommended component split:

| Component | Owns | May do | Must not do |
|---|---|---|---|
| `SimulationRunner` | main execution loop, dequeue step, simulation-time advancement, run lifecycle | dequeue due events, call dispatcher, stop when queue is empty or cancelled | expose the mutable queue to delivery or application layers |
| `ISimulationScheduler` | controlled queue write access | assign `SequenceNumber`, apply `SortRank`, enqueue follow-up events | dequeue events or mutate runtime state directly |
| `IEventDispatcher` | routing one dequeued event to the correct handler pipeline | resolve handler from registry and invoke it | own the main loop or queue ordering |
| `ISimulationEventHandler<TEvent>` | mutation logic for one routed event kind or context | update state and tracking, schedule follow-up events through the scheduler | read or write the raw queue directly |

Recommended access rules:

- writes into the priority queue happen only through `ISimulationScheduler`
- bootstrap scheduling of the first event also goes through `ISimulationScheduler`
- event handlers receive only a handler-facing context and no raw queue access
- reads from the queue for execution happen only in `SimulationRunner`
- diagnostic access, if needed later, should come from derived projections rather than exposed queue internals

## Runtime Flow

```text
SimulationRunner
  -> dequeue next event
  -> advance SimulationState.CurrentTime
  -> dispatch to handler
  -> handler calls orchestrator or specific runtime service
  -> runtime objects mutate local state
  -> scheduler appends follow-up events
  -> snapshot publication happens through snapshot services
```

## `SimulationRunner` Contract Direction

One concrete runtime shape can look like this:

```csharp
public interface ISimulationRunner
{
    Task<SimulationRunResult> RunAsync(
        SimulationExecutionContext context,
        CancellationToken cancellationToken);
}

public sealed class SimulationRunner : ISimulationRunner
{
    public async Task<SimulationRunResult> RunAsync(
        SimulationExecutionContext context,
        CancellationToken cancellationToken)
    {
        while (context.EventQueue.TryDequeue(out var nextEvent))
        {
            cancellationToken.ThrowIfCancellationRequested();

            context.State.AdvanceTo(nextEvent.ScheduledTime);
            await context.Dispatcher.DispatchAsync(
                nextEvent,
                context.CreateHandlerContext(),
                cancellationToken);
        }

        return SimulationRunResult.Completed(context.State.CurrentTime);
    }
}
```

Recommended rule:

- the main dequeue-and-dispatch loop lives in `FlowForge.Simulation`
- the application layer starts or stops the runner through a use case, but does not host the loop itself
- API, CLI, and desktop trigger lifecycle use cases and consume snapshots or results

## Runner Lifecycle Semantics

The runner lifecycle is intentionally narrow, but its behavioral contract should stay explicit.

Current design direction:

- `SimulationRunner` advances the run until the queue is empty, the run is cancelled, or a future explicit control state stops execution
- pause, stop, and reset remain application-facing lifecycle operations and must not be improvised inside handlers
- safe completion means the run ends in a deterministic state once no more executable events remain
- cancellation should stop the loop without corrupting runtime state or published snapshots

Still to be finalized:

- exact pause and resume semantics
- whether paused runs keep a resumable live context or always round-trip through checkpoint state
- how completion, cancellation, and aborted runs are surfaced in `SimulationRunResult`

## Reserved Extension Hooks

The MVP should reserve extension points for later runtime features without overbuilding them now.

Recommended minimum hooks to preserve:

- disturbance entry points such as stage outage, hold, or shipping stop events must fit the same routing and scheduling model as normal runtime events
- replay-oriented features may depend on run markers, ordered snapshot history, or bounded event-history metadata, but should not force full event sourcing into the MVP
- runtime internals should keep enough structure to add disturbance and replay behavior later without replacing queue ownership, dispatching, or tracking boundaries
