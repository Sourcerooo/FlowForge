# Architecture Glossary

This glossary defines the canonical terms used across the architecture documentation.

## Core Terms

- `Order`: the fulfillment-domain business entity used by the initial MVP scenario
- `WorkItem`: the runtime-neutral term for an item moving through the simulation; the fulfillment MVP maps orders to work items
- `Scenario`: the authored input defining one process configuration and run defaults
- `ProcessConfiguration`: the immutable domain-owned runtime configuration derived from a scenario
- `Stage`: a logical process step such as Picking, Packing, or Shipping
- `Station`: a concrete processing resource inside a stage
- `SimulationRun`: one concrete execution of the simulation engine
- `SimulationExecutionContext`: the simulation-internal run-scoped composition root used by runner and dispatcher
- `SimulationExecutionState`: the technical cross-layer document used for checkpoint save and load flows

## Runtime Terms

- `SimulationState`: mutable runtime state owned by the simulation engine
- `SimulationRunner`: the owner of the dequeue-and-dispatch loop
- `ISimulationScheduler`: the only write gateway into the event queue
- `IEventDispatcher`: the routing component that maps one dequeued event to the correct handler
- `EventRoutingKey`: the composite routing key used to resolve stage-aware handlers
- `ProcessingToken`: the run/version marker used to invalidate outdated completion events

## Tracking and Read Model Terms

- `WorkItemTracking`: factual per-work-item timing and segment history
- `StationTracking`: cumulative facts for one concrete station
- `StageTracking`: aggregation of station-level facts to one logical stage
- `IKpiCollector`: owner of compact KPI aggregates and derived metric inputs
- `SimulationSnapshot`: immutable consumer-facing read model produced by the simulation layer
- `Latest Snapshot`: the authoritative current snapshot for live readers
- `Snapshot Timeline`: ordered run-scoped snapshot history for playback-oriented consumers

## Documentation Terms

- `Normative`: describes a binding architecture rule, boundary, or design decision
- `Informative`: explains, illustrates, or tracks state, but is not itself a binding rule
- `Example`: a non-normative sample flow, pseudocode, or reference implementation shape
