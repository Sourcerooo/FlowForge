# 2026-03-17 -- Simulation and Snapshot Direction

## Status

Accepted

## Decisions

- Use immutable snapshots as the primary backend-to-client contract.
- Keep `Created` as the external entry state and use `QueuedForPicking` as the first internal processing state.
- Use a generic queue, start, and completion event family with routing metadata instead of station-specific event types.
- Introduce `GenerateSimulationEvent` as the first scheduled event of a simulation run.
- Add `SequenceNumber`, `SortRank`, and run or version markers to simulation events.
- Track per-station waiting and processing history on each order or order-run structure.
- Publish KPIs as part of the immutable snapshot instead of recalculating them in each consumer.
- Keep one authoritative latest snapshot for live reads and a run-scoped snapshot timeline for playback-oriented consumers.
- Allow references inside snapshots only for run-scoped immutable data such as static scenario or layout information.
- Define explicit snapshot DTOs for scenario, process, stations, active orders, KPIs, alerts, and metadata.
- Keep KPI computation centralized in the simulation layer and publish the results inside `KpiSnapshot`.
- Separate runtime facts into `SimulationState`, `OrderTracking`, `StationTracking`, and compact KPI collector state.
- Update KPI facts incrementally during queue, start, and completion handling and only project final KPI DTOs on snapshot publication.
- Model order history as segments instead of one visit per stage.
- Track stations at concrete `StationId` level and aggregate upward to the logical stage.

## Reasoning

- Immutable snapshots decouple consumers from mutable runtime internals.
- Generic event families and deterministic ordering keep the runtime extensible without sacrificing explicit routing.
- Incremental KPI ownership and segment-based tracking keep snapshot generation efficient and historically meaningful.
- Latest plus timeline storage supports both live monitoring and playback-oriented UX.
