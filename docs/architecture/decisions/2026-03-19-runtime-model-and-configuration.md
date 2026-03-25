# 2026-03-19 -- Runtime Model and Configuration Direction

## Status

Accepted

## Decisions

- Implement `OrderTracking` as a mutable aggregate with read-only segment exposure and transition methods owned by the aggregate.
- Generalize runtime tracking terminology from `Order` to `WorkItem` while keeping fulfillment as the first configured scenario.
- Replace hard-coded `ProcessStage` usage over time with scenario-configured stage and station definitions.
- Separate simulation-facing `TrackingSubjectId` from the domain or master-data reference through a registry structure.
- Use hierarchical JSON scenario files as the MVP source for stage and station topology.
- Use GUIDs for all internal runtime identities and generate them during scenario import instead of storing IDs in JSON.
- Keep `ProcessConfiguration` and its stage and station definitions in `FlowForge.Domain` and let `Simulation` consume that model directly.
- Let `SimulationRunner` own the mutable event queue and main dequeue and dispatch loop, while handlers write to the queue only through `ISimulationScheduler`.
- Create one `SimulationExecutionContext` per simulation run and construct it fully before calling `RunAsync`.
- Allow one concrete `SimulationQueue` implementation to back both `ISimulationEventQueue` and `ISimulationScheduler`, but expose only the scheduler view to handlers.
- Keep checkpoint document models in `FlowForge.Simulation` and place the checkpoint storage port in `FlowForge.Application`.

## Reasoning

- The runtime updates tracking data frequently and needs controlled mutability with clear invariants.
- Config-driven topology supports future process variation without moving canonical process structure out of the domain.
- One run-scoped queue with segregated interfaces preserves deterministic execution while protecting boundaries.
- Keeping checkpoint storage in application and document shapes in simulation preserves dependency direction.
