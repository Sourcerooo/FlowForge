# Application Component

## Purpose

`FlowForge.Application` orchestrates use cases and defines the shared application-facing surface consumed by delivery hosts.

## Responsibilities

- define lifecycle commands and queries such as start, pause, reset, snapshot access, KPI access, save, and load
- orchestrate scenario loading and hand domain-owned process configuration into the simulation layer
- expose shared request, response, result, and validation patterns where they are host-agnostic
- define persistence-facing ports implemented by infrastructure

## Owns

- use-case orchestration
- application-facing ports such as scenario repositories and checkpoint stores
- shared result and error semantics
- validation entry points before runtime orchestration

## Does Not Own

- live runtime mutation logic
- HTTP route definitions, CLI parsing, or desktop view logic
- persistence implementations
- domain business rules that belong in `FlowForge.Domain`

## Boundary Rules

- `Application` may depend on `Simulation` for orchestration-facing contracts and technical save/load documents
- `Application` must not host the simulation main loop
- delivery hosts should reuse application contracts instead of creating parallel command and query models without reason
