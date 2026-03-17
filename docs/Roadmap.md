# Roadmap -- FlowForge

## Purpose

This roadmap describes how FlowForge should evolve from the current repository scaffold into a
digital twin product for fulfillment and logistics process simulation.

It is intentionally broader than `docs/Todo.md`.
The roadmap answers which product and architectural capabilities should come next and in what order.

## Planning Principles

- Deliver visible product value early, not framework complexity.
- Keep the simulation core independent from UI, transport, and persistence.
- Build thin vertical slices that can be demonstrated quickly.
- Favor one strong desktop reference workflow before expanding to multiple clients.
- Document architecture choices before broadening the implementation surface.

## Product Direction

FlowForge starts as a digital twin for a simple fulfillment flow:

```text
Order Source -> Picking -> Packing -> Shipping -> Completed
```

The MVP should let a user:

- start, pause, and reset a simulation
- observe queues, active orders, and worker utilization
- identify bottlenecks visually
- modify a small set of scenario parameters
- understand throughput and lead time changes from those parameters

## Milestones

### Milestone 1 -- Product Foundation

Goal: replace the generic template baseline with a domain-specific simulation foundation.

- define the fulfillment domain language and invariants
- introduce the target solution direction for `Simulation`, `Contracts`, and `Desktop`
- implement the first discrete event runtime for the linear station flow
- validate the engine through tests and CLI/debug execution
- keep the repository documentation aligned with the new product vision

Exit criteria:

- orders can move through the modeled process in code
- the simulation can be executed without UI dependencies
- the core architecture is documented and understandable

### Milestone 2 -- Application Boundary, Snapshot Contract, and Early API

Goal: establish the stable contract between engine and clients.

- expose start, pause, reset, and scenario-loading use cases
- define immutable snapshot DTOs and KPI contracts shared between desktop and API where practical
- introduce KPI collection for throughput, lead time, queue lengths, WIP, and utilization
- formalize application interfaces for persistence and exports
- add early API endpoints for simulation control and snapshot/KPI queries
- add simulation-focused test coverage around runtime transitions and snapshot generation

Exit criteria:

- one application service can drive the simulation lifecycle
- one stable snapshot contract is available for desktop and API consumption
- KPI values can be queried without coupling clients to internal runtime state

### Milestone 3 -- Desktop MVP

Goal: deliver the first demo-worthy client.

- create the desktop shell and main simulation screen
- render stations, queues, and active order flow
- add controls for start, pause, reset, and parameter changes
- present KPI cards and bottleneck highlighting
- support smooth visual updates based on snapshot data

Exit criteria:

- a non-developer can understand the process by using the desktop app
- the product shows live system behavior, not just logs or raw data
- backend and UI evolve through the snapshot contract instead of shared mutable state

### Milestone 4 -- Scenario Management and Replay

Goal: make the MVP reusable and easier to demonstrate.

- add scenario presets and persistence
- support export of run results or KPI summaries
- introduce a simple timeline or replay view if it improves explainability
- improve operational diagnostics and error handling
- harden the workflow for repeated demo usage

Exit criteria:

- scenarios can be saved, loaded, and reused
- simulation output is exportable for analysis or showcasing
- the application supports repeatable demo flows with less manual setup

### Milestone 5 -- Remote and Multi-Client Expansion

Goal: prepare FlowForge for platform-style growth.

- expand the early API into a fuller remote control and query surface
- add realtime streaming for snapshots or live events
- enable a web dashboard or another reference client
- persist richer run history if cross-session analysis becomes valuable
- review authentication, deployment, and operational requirements

Exit criteria:

- the backend can serve at least one out-of-process client
- contracts remain client-neutral and stable
- the product can evolve beyond a single local demo client

### Milestone 6 -- Advanced Digital Twin Features

Goal: move from a compelling MVP toward a richer operational platform.

- add disturbances such as station outages or shipping stops
- support more complex flows such as branching, rework, or priority orders
- add comparative scenario analysis
- extend KPI history and analytical views
- explore optimization and forecasting only after the simulation foundation is stable

Exit criteria:

- the product supports more realistic operational variation
- scenario comparison produces decision-support value
- advanced analytics build on stable simulation primitives instead of ad hoc additions

## Near-Term Release Themes

### Release Theme A -- Make the Engine Real

Focus:

- model the first real domain
- prove the runtime loop
- remove generic-template thinking from the codebase

### Release Theme B -- Make the Boundary Stable

Focus:

- standardize snapshots
- separate simulation state from presentation state
- define how the rest of the product consumes the engine

### Release Theme C -- Make the Product Visible

Focus:

- deliver a polished desktop demo
- show bottlenecks and live flow clearly
- make interaction intuitive enough for feedback sessions

## Risks and Watchpoints

- Over-generalizing too early could delay the first usable demo.
- Building multiple clients before stabilizing the snapshot contract could fragment progress.
- Adding persistence or replay too early could complicate the runtime model.
- Pushing API-first delivery too soon could slow down a desktop-first MVP.
- Weak KPI definitions could make the simulation visually interesting but analytically shallow.

## Open Questions

- Should the first UI investment go exclusively into Avalonia, or should a minimal web dashboard be planned earlier?
- Are disturbances part of the MVP story, or should the first release focus only on normal flow behavior?
- Is scenario persistence enough for the MVP, or is saved run history already needed?
- Does the CLI remain mainly a developer tool, or should it also support operational demo workflows?

## Revision Log

| Date | Change |
|---|---|
| 2026-03-17 | Replaced the generic template roadmap with a product roadmap for the FlowForge digital twin vision. |
