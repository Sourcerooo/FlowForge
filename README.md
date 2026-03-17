# FlowForge

FlowForge is a .NET solution scaffold generated from a Clean Architecture template.
The repository is organized around reusable libraries, executable entry points, automated tests,
containerization assets, and starter documentation so a new project can move from bootstrap to
feature work quickly.

This README gives a concise overview of the solution layout, local setup, and the recommended first
steps after generation.

## Table of Contents

- [Description](#description)
- [Project Vision](#project-vision)
- [Architecture](#architecture)
- [Installation](#installation)
- [Project Layout](#project-layout)
- [Development Workflow](#development-workflow)
- [Documentation](#documentation)
- [Next Steps](#next-steps)

## Description

This repository contains a modular .NET solution with the following goals:

- keep business rules isolated from infrastructure details
- provide clear project boundaries for maintainability and testability
- support multiple entry points such as an API and a CLI
- centralize package versions and build configuration at the repository root
- offer a ready-to-use baseline for CI, Docker, and project documentation

## Project Vision

The generated structure is intended as a practical starting point rather than a rigid framework.
It encourages a solution where domain logic stays independent, application logic coordinates use
cases, infrastructure implements external concerns, and delivery projects expose the system through
specific interfaces.

The template is designed to evolve with the project:

- start small with a minimal but well-structured codebase
- add features without breaking dependency direction
- scale from local development to automated build and test pipelines
- keep architecture decisions visible in documentation instead of tribal knowledge

## Architecture

The solution follows a Clean Architecture style with explicit dependency direction:

```text
Domain
  ^
  |
Application
  ^
  |
Infrastructure
  ^         ^
  |         |
API       CLI

Tests reference the layer they validate.
```

High-level responsibilities:

- `Domain` -- core business concepts, rules, value objects, domain services, domain events
- `Application` -- use cases, orchestration, contracts, validation, DTOs, abstractions
- `Infrastructure` -- persistence, external integrations, messaging, file access, adapters
- `Api` -- HTTP entry point, endpoint composition, transport-specific configuration
- `CLI` -- command-line entry point for automation, jobs, maintenance, or local tooling
- `tests/*` -- layer-focused automated tests

Further details are documented in [`docs/Architecture.md`](docs/Architecture.md).

## Installation

### Prerequisites

- .NET SDK `10.0.200` or a compatible feature-band installation
- Docker Desktop or a compatible Docker runtime if you want to build container images
- GitHub repository access if you want to use the provided CI workflow

### Initial setup

1. Restore dependencies:

```bash
dotnet restore "FlowForge.slnx"
```

2. Build the solution:

```bash
dotnet build "FlowForge.slnx"
```

3. Run the tests:

```bash
dotnet test "FlowForge.slnx"
```

### Run the entry points

Start the API:

```bash
dotnet run --project "src/FlowForge.Api/FlowForge.Api.csproj"
```

Run the CLI:

```bash
dotnet run --project "src/FlowForge.CLI/FlowForge.CLI.csproj"
```

### Container workflow

Build and run the container stack:

```bash
docker compose up --build
```

## Project Layout

| Path | Description |
|---|---|
| `FlowForge.slnx` | Solution file for the full repository |
| `src/FlowForge.Domain/` | Domain layer |
| `src/FlowForge.Application/` | Application layer |
| `src/FlowForge.Infrastructure/` | Infrastructure layer |
| `src/FlowForge.Api/` | HTTP API host and Dockerfile |
| `src/FlowForge.CLI/` | Command-line host and Dockerfile |
| `tests/FlowForge.Domain.Tests/` | Domain tests |
| `tests/FlowForge.Application.Tests/` | Application tests |
| `docs/` | Architecture, roadmap, and task tracking |
| `.github/workflows/ci.yml` | Build, test, and Docker validation in GitHub Actions |
| `docker-compose.yml` | Local orchestration for API and CLI containers |

## Development Workflow

- Keep dependencies pointing inward toward `Domain`
- Add new package versions through `Directory.Packages.props`
- Prefer abstractions in `Application` and implementations in `Infrastructure`
- Add tests alongside the layer that owns the behavior
- Document architectural changes in `docs/Architecture.md`
- Track upcoming work in `docs/Roadmap.md` and `docs/Todo.md`

## Documentation

The `docs/` folder contains project-facing documentation with different purposes:

| Document | Description |
|---|---|
| [`docs/Architecture.md`](docs/Architecture.md) | Current layer model, responsibilities, and extension guidance |
| [`docs/Roadmap.md`](docs/Roadmap.md) | Medium-term milestones and planned capabilities |
| [`docs/Todo.md`](docs/Todo.md) | Working task list for concrete next actions |

## Next Steps

1. Replace the placeholder services and entry-point code with your first real use case.
2. Decide on infrastructure concerns such as persistence, authentication, and external integrations.
3. Expand the tests around the first business-critical domain and application behaviors.
4. Update the documentation so it reflects actual project decisions instead of the initial template.
