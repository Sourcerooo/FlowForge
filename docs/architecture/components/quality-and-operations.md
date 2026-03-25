# Quality and Operations

## Purpose

Quality and operations are cross-cutting capabilities that support reliability, iteration speed, and demo readiness.

## Testing Direction

- keep unit tests close to the owning layer
- add simulation tests for event ordering, state transitions, and KPI behavior
- add integration tests once persistence and API behavior cross real boundaries
- keep demo scenarios repeatable and testable

## Operational Direction

- evolve CI alongside the actual solution structure
- keep Docker and runtime assets aligned with the real hosts in use
- add architecture checks once the target structure is stable enough to enforce
- add structured logging, metrics, and trace strategy as the runtime becomes more complex

## Current Gaps

- simulation test project is still missing
- integration coverage is still missing
- architecture validation rules are not yet automated
- telemetry and release flow are still planned
