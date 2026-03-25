# Simulation Component

## Purpose

`FlowForge.Simulation` is the runtime heart of FlowForge.
It owns mutable execution state and advances the modeled process through discrete events.

## Responsibilities

- own the event queue and main execution loop
- advance simulation time deterministically
- dispatch events to the correct runtime handlers
- mutate runtime state for work items, stations, and stage-level aggregates
- maintain runtime tracking and KPI aggregates
- publish immutable snapshots for API, CLI, desktop, and replay-oriented consumers
- map checkpoint-oriented technical state to and from live runtime state

## Internal Subsystems

- event queue and scheduling
- event dispatcher and handler registry
- simulation state and execution context
- work-item, station, and stage tracking
- KPI collector
- snapshot builder, latest snapshot store, and snapshot timeline store
- checkpoint mapping between live runtime and technical documents

## Owns

- mutable runtime state
- event ordering and dispatch semantics
- runtime tracking structures
- snapshot publication lifecycle
- simulation-internal orchestration boundaries such as `IWorkItemProcessOrchestrator`

## Does Not Own

- business process topology as the canonical source of truth
- application use-case orchestration
- persistence adapters and file formats as infrastructure implementations
- delivery-specific interaction behavior

## Boundary Rules

- the raw event queue is runner-owned and must not be exposed to handlers or delivery hosts
- handlers write follow-up events only through `ISimulationScheduler`
- readers observe immutable snapshots instead of mutable runtime collections
- checkpoint document shapes may live here, but storage adapters do not
