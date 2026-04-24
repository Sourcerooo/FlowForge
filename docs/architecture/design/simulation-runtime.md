# Simulation Runtime Design

This document is the entry point for the runtime design of `FlowForge.Simulation`.
It defines the runtime at a high level and links to the more specific design documents.

## Runtime Overview

The simulation runtime follows a deterministic dequeue-and-dispatch loop.

```text
Start simulation
  -> create run-scoped execution context
  -> schedule first event
  -> runner dequeues next due event
  -> dispatcher resolves the responsible handler
  -> handler or orchestrator mutates runtime state
  -> scheduler appends follow-up events
  -> snapshot services publish immutable read models
```

Core runtime rules:

- `SimulationRunner` owns the dequeue-and-dispatch loop.
- `ISimulationScheduler` is the only write gateway into the queue.
- `IEventDispatcher` resolves handlers from the DI-registered handler set.
- handlers stay thin and delegate multi-object process steps to orchestration boundaries.
- runtime consumers observe immutable snapshots, not mutable runtime internals.

## Runtime Design Map

Use the following files for concrete runtime topics:

- `docs/architecture/design/simulation-runner.md` for runtime loop, queue ownership, ordering, and lifecycle semantics
- `docs/architecture/design/simulation-events.md` for event family, base event contract, ordering semantics, and invalidation rules
- `docs/architecture/design/simulation-dispatching.md` for dispatcher contracts, DI-based handler resolution, and dispatch handoff
- `docs/architecture/design/simulation-execution-context.md` for execution context shape, handler-facing context, and run factory direction
- `docs/architecture/design/simulation-orchestration.md` for runtime object boundaries, process orchestration, commands, and flow sequencing

## Relationship To Other Design Documents

- `docs/architecture/design/scenario-configuration.md` defines the canonical process configuration and tracking structures consumed by the runtime.
- `docs/architecture/design/snapshots-and-kpis.md` defines snapshot publication and KPI ownership used by the runtime.
- `docs/architecture/design/checkpoints.md` defines checkpoint-oriented technical contracts for save and load flows.
