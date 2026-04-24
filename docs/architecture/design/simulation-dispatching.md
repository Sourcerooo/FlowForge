# Simulation Dispatching Design

This document is normative for dispatcher behavior, handler resolution, dependency injection based registration, and dispatch handoff.

## Resolution Model

The dispatcher resolves handlers by runtime event kind.

Recommended shape:

```text
EventKind -> ISimulationEventHandler
```

Handlers declare the event kind they own through `CanHandle()`.

Recommended dispatcher behavior:

- inspect `simulationEvent.EventKind`
- resolve the matching handler from the DI-registered handler set
- fail fast for missing required handlers
- keep handler lookup outside `SimulationExecutionContext`

## Dispatcher Contract Direction

Recommended handler contract shape:

```csharp
public interface ISimulationEventHandler
{
    EventKind CanHandle();
    Task Process(
        SimulationEvent simulationEvent,
        SimulationExecutionHandlerContext context,
        CancellationToken cancellationToken);
}
```

## Registration Flow

- handler instances are registered through dependency injection
- the dispatcher receives the registered `IEnumerable<ISimulationEventHandler>` through dependency injection
- the dispatcher materializes its internal lookup from the injected handlers during construction
- the dispatcher performs lookup and invocation only
- missing required handlers fail fast because silent dropping would corrupt the run

Recommended handler resolution model:

- each concrete runtime event kind should resolve to exactly one handler
- `CanHandle()` is the stable declaration of ownership for one handler implementation
- duplicate handler registrations for the same event kind are invalid composition

## Dispatch Handoff

1. `SimulationRunner` dequeues the next due event.
2. `SimulationRunner` advances `SimulationState.CurrentTime`.
3. `SimulationRunner` calls `IEventDispatcher.DispatchAsync(event, handlerContext, cancellationToken)`.
4. The dispatcher resolves the handler from the injected handler set by `EventKind`.
5. The resolved handler mutates runtime state and schedules follow-up events through `ISimulationScheduler`.
