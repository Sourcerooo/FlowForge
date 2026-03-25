# Simulation Dispatching Design

This document is normative for routing keys, dispatcher behavior, handler resolution, and dispatch handoff.

## Routing Model

The dispatcher should route by a composite key instead of CLR type only.

Recommended shape:

```text
EventKind + StageId or ProcessStage + OptionalSubKind
```

Examples:

- `WorkItemQueue + Picking`
- `ProcessingStart + Packing`
- `ProcessingComplete + Shipping`
- `DisruptionRaise + Picking`

Recommended dispatcher behavior:

- identify the generic event family
- inspect stage or process metadata
- resolve the matching handler from an immutable registry
- fail fast for missing required handlers
- receive the registry itself through dependency injection instead of pulling it from `SimulationExecutionContext`

## Dispatcher Contract Direction

Recommended routing key shape:

```csharp
public readonly record struct EventRoutingKey(
    EventKind EventKind,
    ProcessStage ProcessStage,
    string? SubKind = null);

public interface IEventHandlerRegistry
{
    ISimulationEventHandler Resolve(EventRoutingKey key);
}
```

## Registration Flow

- handler instances are registered through dependency injection
- the registry is built from the registered handler instances when the runtime composition is created
- the registry is immutable for the full lifetime of the run
- the dispatcher performs lookup and invocation only
- missing required handlers fail fast because silent dropping would corrupt the run

Recommended handler resolution model:

- the registry key should be `EventRoutingKey(EventKind, StageId or ProcessStage, SubKind)`
- exact-match lookup is the default behavior for required runtime events
- optional fallback handlers may be allowed only for stage-agnostic events such as `Generate + None` or `SnapshotPublished + None`

## Dispatch Handoff

1. `SimulationRunner` dequeues the next due event.
2. `SimulationRunner` advances `SimulationState.CurrentTime`.
3. `SimulationRunner` calls `IEventDispatcher.DispatchAsync(event, handlerContext, cancellationToken)`.
4. The dispatcher resolves the handler from `EventRoutingKey`.
5. The resolved handler mutates runtime state and schedules follow-up events through `ISimulationScheduler`.
