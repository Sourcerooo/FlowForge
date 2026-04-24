# 2026-03-25 -- In-Memory Runtime Slice and Context DI Boundary

## Status

Accepted

## Decisions

- Keep the current runtime slice in-memory first and defer scenario and checkpoint persistence until the core simulation flow is stable.
- Allow scenario data to be loaded fresh on application start for now instead of treating persistence as part of the current MVP-critical path.
- Remove generic service aggregation from `SimulationExecutionContext` and keep it focused on run-scoped execution data.
- Inject dispatcher, scheduler, event handlers, orchestrators, KPI collectors, and similar collaborators into the runtime classes that own the corresponding behavior.

## Reasoning

- The current branch already proves queue, event, dispatcher, and runner primitives, but persistence would add format and mapping pressure before the runtime slice is stable.
- A data-focused execution context makes runtime ownership clearer and avoids passing a mutable service bag through the event pipeline.
- Constructor-injected collaborators fit Clean Architecture boundaries better and keep responsibilities explicit in runner, dispatcher, and handler implementations.
