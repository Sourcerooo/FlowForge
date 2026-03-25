# Application Contracts Design

This document is normative for application-facing use cases, shared contracts, and validation direction.

## Application Use Cases

The application layer should expose explicit use cases for:

- simulation lifecycle: start, pause, reset, status retrieval
- scenario loading and configuration selection
- snapshot queries
- KPI queries
- checkpoint save and load
- later disturbance commands if they are accepted into scope

## Shared Contract Direction

Current direction:

- shared DTOs and technical contract types are still needed, but a dedicated contract project is not yet required
- snapshot contracts should stay close to their owning layer until a separate assembly is justified
- request models may be shared where desktop and API truly need the same semantics

## Result and Error Model

Application use cases should expose consistent success and failure semantics across delivery hosts.

Recommended categories:

- validation failure
- not found
- concurrency or stale state conflict
- runtime execution failure
- infrastructure failure mapped into application-friendly errors

## Validation Direction

- validate input before entering runtime orchestration where practical
- keep validation rules close to the owning use case or request model
- map validation failures consistently for API, CLI, and later desktop flows

## Shared Snapshot Direction

Desktop and API should rely on the same core snapshot schema when feasible.
Transport-specific wrappers may exist, but the central read model should stay aligned.
