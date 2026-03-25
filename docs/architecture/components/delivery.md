# Delivery Components

## Purpose

The delivery layer exposes FlowForge through concrete hosts.
Desktop is the primary MVP experience, but API support starts early and the CLI remains valuable for debugging and operations.

## Desktop Host

Primary MVP client responsibilities:

- visualize stations, queues, and work-item flow
- display KPI cards and charts based on immutable snapshots
- offer simulation controls such as start, pause, reset, and scenario selection
- highlight bottlenecks and notable system states

Desktop must remain a thin consumer of application-facing contracts and published snapshots.

## API Host

Responsibilities:

- expose control and query endpoints over HTTP
- forward commands and queries into application use cases
- return shared DTOs or mapped transport models without leaking runtime internals

The API stays thin and must not host business rules or simulation mutation logic.

## CLI Host

Responsibilities:

- provide debug-oriented runtime execution before the desktop exists
- support admin and export workflows
- expose deterministic command-line driven runs and inspections

The CLI is a delivery host, not a parallel orchestration layer.

## Shared Delivery Rules

- delivery hosts consume immutable snapshots and application-facing contracts
- delivery hosts do not bind directly to `SimulationState` or mutable runtime collections
- delivery hosts should share contracts where practical instead of drifting into separate product models
