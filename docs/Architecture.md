# Architecture -- FlowForge

This document is the normative high-level architecture overview for FlowForge.
It describes the product scope, tech stack, architectural boundaries, main components, and dependency rules.
It does not define detailed process flows, interface signatures, or example implementations.

For detailed design and implementation guidance, continue in `docs/architecture/index.md`.

## Product Focus

FlowForge is a digital twin application for operational process flows.
The first product slice models a fulfillment pipeline that is visually clear, domain-specific, and small enough for iterative delivery.

Initial MVP flow:

```text
Order Source -> Picking -> Packing -> Shipping -> Completed
```

FlowForge should make the following operational behavior visible:

- order movement through stations
- queue growth and shrinkage
- worker utilization per station
- throughput and lead time behavior
- bottleneck formation under changing parameters

## Documentation Structure

The architecture documentation is split by responsibility and level of detail.

| Document area | Purpose | Normative |
|---|---|---|
| `docs/Architecture.md` | High-level architecture, tech stack, components, dependency rules | Yes |
| `docs/architecture/index.md` | Navigation, reading order, and document map | Yes |
| `docs/architecture/components/*.md` | Component responsibilities and boundaries | Yes |
| `docs/architecture/design/*.md` | Technical design, interfaces, state boundaries, data flows | Yes |
| `docs/architecture/examples/*.md` | Example flows, pseudocode, and reference shapes | No |
| `docs/architecture/decisions/*.md` | Architecture decisions currently in force | Yes |
| `docs/architecture/glossary.md` | Shared architectural vocabulary | Yes |
| `docs/architecture/open-questions.md` | Open architecture and design questions | Informative |
| `docs/architecture/current-state.md` | Current implementation maturity snapshot | Informative |

## Tech Stack

- Language and runtime: C# on .NET using the SDK and framework versions defined in `global.json`
- Architecture style: Clean Architecture with explicit inward dependency direction
- Core execution model: event-driven simulation runtime with immutable read snapshots
- Delivery hosts: API, CLI, and a WPF desktop application
- Persistence direction: in-memory-first runtime for the early MVP, with scenario and checkpoint persistence added after the core runtime stabilizes
- Testing direction: unit tests close to the owning layer, with integration tests added where boundaries are crossed

## High-Level Architecture

```text
+----------------------------------------------------------------------------+
|                              DELIVERY LAYER                                |
|                                                                            |
|  Desktop Client         API Host               CLI Host                    |
|  Visualization          Control and queries    Debug and admin workflows    |
+----------------------------------------------------------------------------+
|                         INFRASTRUCTURE LAYER                               |
|                                                                            |
|  Scenario persistence   Checkpoint persistence Export and diagnostics       |
|  Technical adapters     JSON mapping           Operational support          |
+----------------------------------------------------------------------------+
|                     APPLICATION AND CONTRACTS                              |
|                                                                            |
|  Use-case orchestration Queries and commands   Shared DTO/result contracts  |
+----------------------------------------------------------------------------+
|                           SIMULATION LAYER                                 |
|                                                                            |
|  Event runtime          Mutable execution      Snapshots and KPI services   |
|  Scheduling             Dispatching            Runtime tracking             |
+----------------------------------------------------------------------------+
|                             DOMAIN LAYER                                   |
|                                                                            |
|  Process configuration  Business concepts      Invariants and vocabulary    |
+----------------------------------------------------------------------------+
```

High-level interaction model:

```text
Desktop Client   API Host   CLI Host
       \            |          /
        \           |         /
         +----------v--------+
         |  Application +    |
         |    Contracts      |
         +----+---------+----+
              |         |
              |         +------------------+
              v                            v
      +-------+--------+         +---------+--------+
      | Simulation     |         | Infrastructure   |
      | event runtime  |         | persistence/etc. |
      +-------+--------+         +------------------+
              |
              v
      +-------+--------+
      | Domain         |
      | business model |
      +----------------+
```

## Core Architecture Rules

- Respect Clean Architecture boundaries.
- Preserve dependency direction: `Domain <- Simulation <- Application <- Infrastructure <- Delivery`.
- `Domain` owns business concepts and process configuration, not transport or persistence concerns.
- `Simulation` owns mutable runtime state and the main event loop.
- `Application` orchestrates use cases and shared contracts; it does not own runtime mutation logic.
- `Infrastructure` implements technical adapters, later persistence and serialization concerns, and operational services.
- Delivery hosts stay thin and delegate behavior inward.
- UI and API consumers must read immutable snapshots and must never bind directly to mutable simulation internals.
- The MVP remains concrete and fulfillment-oriented instead of becoming a generic simulation platform too early.

## Component Overview

- `FlowForge.Domain`: process configuration, domain concepts, value objects, and invariants
- `FlowForge.Simulation`: event queue, runner, dispatcher, runtime state, tracking, KPIs, snapshot publication
- `FlowForge.Application`: start/query/save/load use cases, result models, validation, and ports
- `FlowForge.Infrastructure`: scenario loading and checkpoint storage when introduced, export, mapping, diagnostics support
- `FlowForge.Api`: thin HTTP host for control and query access
- `FlowForge.CLI`: thin command-line host for debug, admin, and operational workflows
- `FlowForge.UiWpf`: current WPF desktop visualization host for the MVP path

Detailed responsibilities live in:

- `docs/architecture/components/domain.md`
- `docs/architecture/components/simulation.md`
- `docs/architecture/components/application.md`
- `docs/architecture/components/infrastructure.md`
- `docs/architecture/components/delivery.md`
- `docs/architecture/components/quality-and-operations.md`

## Runtime Model

The runtime follows a simple high-level pattern:

```text
Start simulation
  -> create run-scoped execution context
  -> schedule first event
  -> runner dequeues next due event
  -> dispatcher resolves the responsible handler
  -> handler/orchestrator mutates runtime state
  -> scheduler appends follow-up events
  -> snapshot services publish immutable read models
```

Detailed runtime, event, snapshot, configuration, and checkpoint design lives in:

- `docs/architecture/design/simulation-runtime.md`
- `docs/architecture/design/simulation-runner.md`
- `docs/architecture/design/simulation-events.md`
- `docs/architecture/design/simulation-dispatching.md`
- `docs/architecture/design/simulation-execution-context.md`
- `docs/architecture/design/simulation-orchestration.md`
- `docs/architecture/design/snapshots-and-kpis.md`
- `docs/architecture/design/scenario-configuration.md`
- `docs/architecture/design/application-contracts.md`
- `docs/architecture/design/checkpoints.md`

## Dependency Rules

Current generated references:

```text
FlowForge.Simulation        -> FlowForge.Domain
FlowForge.Application       -> FlowForge.Domain, FlowForge.Simulation
FlowForge.Infrastructure    -> FlowForge.Application, FlowForge.Domain
FlowForge.Api               -> FlowForge.Application, FlowForge.Infrastructure
FlowForge.CLI               -> FlowForge.Domain, FlowForge.Application, FlowForge.Infrastructure, FlowForge.Simulation
FlowForge.Domain.Tests      -> FlowForge.Domain
FlowForge.Application.Tests -> FlowForge.Application
```

Target direction to preserve while the product grows:

```text
FlowForge.Domain
  <- FlowForge.Simulation
     <- FlowForge.Application
        <- FlowForge.Infrastructure
           <- FlowForge.Api / FlowForge.CLI / FlowForge.UiWpf
```

Rules to preserve:

- `Domain` must stay independent.
- `Simulation` must not depend on delivery hosts.
- `Application` may depend on `Simulation` for orchestration-facing contracts and ports, but not for live runtime mutation internals.
- `Infrastructure` implements application-facing ports and must not become the owner of business policy.
- Delivery hosts should share application-facing contracts where practical instead of inventing separate core models.

## Repository Structure

Current repository structure:

```text
.
|-- AGENTS.md
|-- FlowForge.slnx
|-- Directory.Build.props
|-- Directory.Packages.props
|-- global.json
|-- src/
|   |-- FlowForge.Domain/
|   |-- FlowForge.Simulation/
|   |-- FlowForge.Application/
|   |-- FlowForge.Infrastructure/
|   |-- FlowForge.Api/
|   `-- FlowForge.CLI/
|-- tests/
|   |-- FlowForge.Domain.Tests/
|   `-- FlowForge.Application.Tests/
|-- docs/
|   |-- Architecture.md
|   |-- architecture/
|   |-- Roadmap.md
|   `-- Todo.md
`-- .github/workflows/
```

Target product-oriented structure:

```text
src/
  FlowForge.Domain
  FlowForge.Simulation
  FlowForge.Application
  FlowForge.Infrastructure
  FlowForge.Api
  FlowForge.CLI
  FlowForge.UiWpf

tests/
  FlowForge.Domain.Tests
  FlowForge.Simulation.Tests
  FlowForge.Application.Tests
```

## Related Documents

- Start at `docs/architecture/index.md` for the reading guide.
- Use `docs/architecture/glossary.md` for canonical terms.
- Use `docs/architecture/decisions/` for active architecture decisions.
- Use `docs/architecture/open-questions.md` for unresolved issues.
- Use `docs/architecture/current-state.md` for implementation maturity tracking.
