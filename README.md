# FlowForge

FlowForge is a digital twin and simulation project for operational process flows.
The first product focus is a fulfillment pipeline where orders move through picking, packing, and
shipping while the system exposes queues, utilization, throughput, and bottlenecks through a visual
client.

This repository currently contains the initial .NET solution scaffold and the first architecture
documents for evolving it into that product.

## Table of Contents

- [Vision](#vision)
- [MVP Scope](#mvp-scope)
- [Architecture Direction](#architecture-direction)
- [Current Repository State](#current-repository-state)
- [Planned Solution Evolution](#planned-solution-evolution)
- [Development Setup](#development-setup)
- [Documentation](#documentation)
- [Next Decisions](#next-decisions)

## Vision

FlowForge is meant to become a visually strong and technically clean simulation application that
makes operational flow behavior understandable.

The initial domain is intentionally concrete:

```text
Order Source -> Picking -> Packing -> Shipping -> Completed
```

The product should help answer questions such as:

- Where is the current bottleneck?
- How do queue lengths change over time?
- What happens if processing times or worker counts change?
- Which station is overloaded?
- How long does an order need from entry to completion?

The project is optimized for a small team, fast visible progress, and a strong demo path.
The primary experience is desktop-first, but an API is planned early so both delivery paths can
reuse the same core contracts and use cases.

## MVP Scope

The MVP should provide:

- a discrete event simulation of the linear fulfillment process
- configurable worker counts and processing times per station
- immutable simulation snapshots as the backend-to-client contract
- KPI tracking for throughput, lead time, WIP, queue lengths, and utilization
- controls for start, pause, and reset
- a desktop visualization that shows station state, active orders, and bottlenecks
- an early API for shared control and query access

The MVP explicitly avoids premature complexity such as microservices, forecasting, generic plugin
engines, or advanced optimization.

## Architecture Direction

FlowForge follows a Clean Architecture style, extended with a dedicated simulation runtime.

Target dependency direction:

```text
Domain <- Simulation <- Application <- Infrastructure <- Delivery
```

Key idea:

- the simulation owns the mutable runtime state
- clients never bind directly to mutable simulation internals
- UI and external consumers work with immutable snapshots and use-case boundaries

Target high-level modules:

- `FlowForge.Domain` -- domain concepts and business rules
- `FlowForge.Simulation` -- discrete event engine and runtime state
- `FlowForge.Application` -- use cases and orchestration
- `FlowForge.Infrastructure` -- persistence, configuration, logging, exports
- `FlowForge.Desktop` -- primary MVP client
- `FlowForge.Api` -- early control and query host that should align with desktop contracts
- `FlowForge.CLI` -- debug, admin, and automation workflows

Further detail is documented in `docs/Architecture.md`.

## Current Repository State

The repository currently contains these projects:

- `src/FlowForge.Domain/`
- `src/FlowForge.Simulation/`
- `src/FlowForge.Application/`
- `src/FlowForge.Infrastructure/`
- `src/FlowForge.Api/`
- `src/FlowForge.CLI/`
- `tests/FlowForge.Domain.Tests/`
- `tests/FlowForge.Application.Tests/`

This means the documented product direction is ahead of the current code structure in some areas.
That is intentional: the documentation now defines the target shape that the implementation should
grow toward.

## Planned Solution Evolution

Near-term structural additions:

- `src/FlowForge.Desktop/`
- `tests/FlowForge.Simulation.Tests/`

Recommended implementation sequence:

1. Model the fulfillment domain and simulation runtime.
2. Stabilize the snapshot and KPI contract.
3. Build the first desktop visualization client.
4. Add scenario persistence and replay/export support.
5. Expand the early API further and move into broader multi-client delivery when the core demo is stable.

## Development Setup

### Prerequisites

- .NET SDK `10.0.200`
- Docker Desktop or a compatible Docker runtime for container workflows

### Basic commands

Restore dependencies:

```bash
dotnet restore "FlowForge.slnx"
```

Build the solution:

```bash
dotnet build "FlowForge.slnx"
```

Run tests:

```bash
dotnet test "FlowForge.slnx"
```

Run the current API host:

```bash
dotnet run --project "src/FlowForge.Api/FlowForge.Api.csproj"
```

Run the current CLI host:

```bash
dotnet run --project "src/FlowForge.CLI/FlowForge.CLI.csproj"
```

## Documentation

- `docs/Architecture.md` -- target architecture, runtime model, boundaries, and key decisions
- `docs/Roadmap.md` -- milestone-level product and architecture evolution
- `docs/Todo.md` -- operational task list and pending architecture decisions
- `docs/brainstorm/` -- source material, idea sketches, and mockups that informed the current direction

## Next Decisions

The most important open decisions at the moment are:

1. which contracts should be shared directly between desktop and API versus mapped per delivery host
2. whether scenario persistence remains file-based for the MVP
3. whether disturbances belong in the MVP or the first post-MVP expansion
