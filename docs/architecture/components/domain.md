# Domain Component

## Purpose

`FlowForge.Domain` owns the stable business language of FlowForge.
It models the process configuration and domain concepts independently from simulation runtime, delivery hosts, persistence, and transport concerns.

## Responsibilities

- define core business concepts such as orders, scenarios, stages, stations, and capacities
- own immutable process configuration and related invariants
- provide strongly typed value objects instead of primitive-heavy modeling where appropriate
- protect business rules from leakage into delivery and infrastructure layers
- remain concrete enough for the fulfillment MVP while leaving safe extension points for richer process flows

## Owns

- process configuration concepts such as `ProcessConfiguration`, `StageDefinition`, `StationDefinition`, and arrival profile definitions
- domain identities such as `OrderId`, `ScenarioId`, `StageId`, and `StationId` if typed wrappers are introduced
- business invariants related to stage ordering, station ownership, and scenario validity
- canonical fulfillment vocabulary used across the rest of the system

## Does Not Own

- mutable event runtime state
- UI-facing read models and snapshot DTOs
- HTTP, CLI, desktop, or persistence-specific concerns
- checkpoint file shapes and serialization logic

## Near-Term Direction

- replace placeholder structure with concrete fulfillment concepts and invariants
- keep process topology in the domain instead of duplicating it in the simulation layer
- define clear value objects and naming conventions once the first concrete model lands
