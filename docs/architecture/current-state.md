# Current Architecture State

This document summarizes the current implementation maturity of the architecture.
It is informative and should not replace `docs/Todo.md` for task planning.

## Layer Snapshot

| Layer | State | Notes |
|---|---|---|
| Domain | Starter baseline exists | Core fulfillment and process-configuration model still needs to be implemented concretely |
| Simulation | Partial implementation exists | Event base types, queue primitives, dispatcher baseline, and runner loop exist; handler registration, context refactoring, and tests are still pending |
| Application | Baseline exists | Use cases, contracts, and validation are still thin |
| Infrastructure | Baseline exists | Persistence stays intentionally deferred while the current runtime remains in-memory; export is still pending |
| API | Host exists | Still a placeholder control/query surface |
| CLI | Host exists and usable | Best current place for early runtime debugging |
| Desktop | Prototype exists | `FlowForge.UiWpf` provides a first mockup-aligned WPF shell; live application integration is still pending |
| Testing | Domain and application test projects exist | Simulation and integration coverage are still missing |

## Component Snapshot

| Component | State | Main gap |
|---|---|---|
| Domain modeling | Partial | Concrete business concepts, invariants, and value objects |
| Event runtime | Partial | Queue and runner baselines exist; handler registration, run-scoped wiring, vertical paths, and tests remain |
| Snapshots and KPIs | Direction defined | Concrete DTOs, publication cadence, and collector implementation |
| Scenario loading | Deferred | JSON schema validation and mapping into domain configuration start after the in-memory runtime slice stabilizes |
| Application use cases | Planned | Lifecycle commands, snapshot/KPI queries, save/load orchestration |
| Checkpoints | Designed | First storage implementation and resume flow |
| API surface | Partial | Simulation-specific endpoints and shared mapping rules |
| CLI workflows | Partial | Concrete debug, export, and admin commands |

## Capability Detail Snapshot

### Domain

| Capability | State | Main gap |
|---|---|---|
| Order model | Planned | Identity, lifecycle, timestamps, and allowed state transitions |
| Station and work-center model | Planned | Capacity semantics, queue behavior, naming, and ordering |
| Scenario model | Planned | Scenario identity, configuration defaults, and processing parameters |
| Processing profile | Planned | Deterministic versus stochastic timing and parameter model |
| Worker capacity model | Planned | Concurrency semantics, utilization meaning, and invariants |
| Strongly typed value objects | Planned | Domain-specific identities and value wrappers |
| Shared business vocabulary | Planned | Canonical names for orders, work items, stages, stations, and capacities |
| Invariant enforcement | Planned | Move rules into entities and value objects instead of hosts or handlers |

### Infrastructure, Delivery, and Operations

| Capability | State | Main gap |
|---|---|---|
| Scenario loader and repository | Deferred | JSON schema validation, directory layout, and first repository adapter follow the in-memory-first runtime slice |
| Export ownership and format | Planned | Application-facing export port and first export format |
| Replay storage scope | Planned | Decide whether replay starts with snapshots, summaries, or deeper history |
| Desktop snapshot consumption | Partial | WPF shell exists, but it still needs application-driven snapshot binding and lifecycle commands |
| Demo scenario verification | Planned | Repeatable fixtures and expected KPI or process outcomes |
| Packaging and release path | Planned | Delivery packaging strategy for desktop, API, CLI, and scenario assets |

## Delivery Direction

- Desktop remains the primary MVP experience.
- API support starts early for shared control and query access.
- CLI remains the practical early host for deterministic debugging and operational workflows.
