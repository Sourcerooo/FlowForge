# Architecture -- FlowForge

## Table of Contents

- [1. Target Architecture](#1-target-architecture)
  - [1.1 Legend](#11-legend)
  - [1.2 Layer Overview with Status](#12-layer-overview-with-status)
- [2. Capability Details and Feature List](#2-capability-details-and-feature-list)
  - [2.1 Domain Layer](#21-domain-layer)
  - [2.2 Application Layer](#22-application-layer)
  - [2.3 Infrastructure Layer](#23-infrastructure-layer)
  - [2.4 Delivery Layer](#24-delivery-layer)
  - [2.5 Quality and Operations](#25-quality-and-operations)
- [3. Dependency Rules](#3-dependency-rules)
- [4. Request and Execution Flows](#4-request-and-execution-flows)
- [5. Repository Structure](#5-repository-structure)
- [6. Architecture Decision Log](#6-architecture-decision-log)

---

## 1. Target Architecture

This repository starts as a layered .NET solution based on Clean Architecture. The main goal is to
keep business logic independent from frameworks, infrastructure providers, transport mechanisms,
and operational tooling.

### 1.1 Legend

- 🟢 **GREEN** -- feature is present and usable
- 🟡 **YELLOW** -- feature exists in a starter form and should be expanded
- 🔴 **RED** -- feature is planned but not implemented yet

### 1.2 Layer Overview with Status

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│                           DELIVERY LAYER                                   │
│                                                                             │
│  🟢 API Host              🟡 CLI Commands         🔴 Additional Hosts        │
│  🟢 Health Endpoint       🔴 Auth Flows           🔴 Background Workers      │
├─────────────────────────────────────────────────────────────────────────────┤
│                        INFRASTRUCTURE LAYER                                │
│                                                                             │
│  🟡 Dependency Wiring     🔴 Persistence          🔴 External Providers      │
│  🔴 Messaging             🔴 File Storage         🔴 Caching                 │
├─────────────────────────────────────────────────────────────────────────────┤
│                         APPLICATION LAYER                                  │
│                                                                             │
│  🟡 Use Case Shells       🟡 DI Extensions        🔴 Validation Pipeline     │
│  🔴 Commands / Queries    🔴 Result Mapping       🔴 Policies / Behaviors    │
├─────────────────────────────────────────────────────────────────────────────┤
│                           DOMAIN LAYER                                     │
│                                                                             │
│  🟡 Assembly Marker       🔴 Entities             🔴 Value Objects           │
│  🔴 Domain Services       🔴 Domain Events        🔴 Business Rules          │
├─────────────────────────────────────────────────────────────────────────────┤
│                        QUALITY AND OPERATIONS                              │
│                                                                             │
│  🟢 Unit Test Projects    🟢 GitHub Actions       🟢 Dockerfiles             │
│  🟢 Docker Compose        🔴 Integration Tests    🔴 Observability           │
└─────────────────────────────────────────────────────────────────────────────┘
```

Dependency direction:

```text
Domain <- Application <- Infrastructure <- Delivery
```

Delivery projects may compose services from outer layers, but business rules should remain inside
`Domain` and `Application`.

---

## 2. Capability Details and Feature List

Each section below lists the expected responsibilities of the layer, the current starter state, and
what is still missing before the project becomes production-ready.

---

## 2.1 Domain Layer

The domain layer should become the most stable part of the solution. It must remain independent
from transport, persistence, and framework-specific implementation details.

### 2.1.1 Core Domain Modeling

> Business concepts, invariants, and domain language.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Domain assembly structure | 🟡 | The project exists and provides a stable location for business logic. | Replace the assembly marker with real domain types and business concepts. |
| Entities | 🔴 | Rich domain entities should model identity and lifecycle. | Add concrete entities with meaningful invariants and behaviors. |
| Value objects | 🔴 | Value objects should capture important concepts without identity. | Introduce strongly typed value objects instead of primitive-heavy models. |
| Domain services | 🔴 | Domain services should hold business logic that does not belong to a single entity. | Define services only when logic cannot naturally live inside entities or value objects. |
| Domain events | 🔴 | Domain events should communicate important business state changes. | Decide event style, dispatch strategy, and integration with the application layer. |

### 2.1.2 Domain Quality and Boundaries

> Rules that keep the domain independent and expressive.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Framework independence | 🟡 | The starter project has no framework-heavy domain code. | Preserve this rule as the model grows and avoid leaking ORM, HTTP, or UI concerns. |
| Ubiquitous language | 🔴 | Domain naming should reflect the actual business vocabulary. | Align namespaces, classes, and methods with the real domain language. |
| Invariant enforcement | 🔴 | Domain objects should guard valid state transitions. | Move validation and business rules into the model instead of leaving them in handlers. |

---

## 2.2 Application Layer

The application layer coordinates use cases and defines contracts for infrastructure. It should not
contain transport-specific behavior or technical implementation details.

### 2.2.1 Use Cases and Orchestration

> Application-specific workflows that drive the system.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Dependency injection entry point | 🟡 | A starter `AddApplication()` extension method exists. | Register real handlers, validators, policies, and application services. |
| Commands and queries | 🔴 | Use cases should be represented as explicit application operations. | Add request models, handlers, and orchestration per business workflow. |
| DTOs and contracts | 🔴 | Application models should decouple the core from delivery concerns. | Create input and output contracts for meaningful use cases. |
| Validation pipeline | 🔴 | Validation should happen before business execution where appropriate. | Introduce validation rules and a consistent way to surface failures. |
| Result handling | 🔴 | Use cases should return clear success and failure outcomes. | Define result patterns, error types, or exception boundaries. |

### 2.2.2 Cross-Cutting Policies

> Shared behaviors applied across multiple use cases.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Transaction boundaries | 🔴 | Application workflows often need coordinated persistence boundaries. | Define unit-of-work or transaction patterns if the project requires them. |
| Authorization policies | 🔴 | Authorization should be enforced close to the use case boundary. | Add policy abstractions and integrate them in delivery projects. |
| Pipeline behaviors | 🔴 | Logging, validation, timing, and auditing often benefit from shared wrappers. | Introduce middleware or pipeline behaviors if the application grows. |

---

## 2.3 Infrastructure Layer

The infrastructure layer implements technical details required by the application while depending on
contracts defined closer to the core.

### 2.3.1 Technical Adapters

> Integrations with storage, messaging, and external systems.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Dependency injection entry point | 🟡 | A starter `AddInfrastructure()` extension method exists. | Register concrete adapters and connect configuration-backed implementations. |
| Persistence adapters | 🔴 | Repositories and data access should live here. | Choose a data store, define persistence models, and implement repositories. |
| External service clients | 🔴 | APIs, queues, and third-party systems belong in infrastructure. | Add provider-specific clients and resilience policies. |
| File and blob storage | 🔴 | File-system or object-storage abstractions belong here. | Define interfaces in `Application` and implement the chosen storage provider. |
| Caching | 🔴 | Performance-oriented technical caching belongs here. | Decide whether in-memory, distributed, or no caching is appropriate. |

### 2.3.2 Operational Integration

> Infrastructure responsibilities that affect runtime behavior and deployment.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Configuration-backed wiring | 🔴 | Implementations often depend on environment-specific settings. | Add option classes, configuration sections, and validation. |
| Messaging and async processing | 🔴 | Event-driven or background integration may be needed later. | Introduce queues, buses, or schedulers only when requirements justify them. |
| Observability hooks | 🔴 | Logging, tracing, and metrics are often initialized here. | Add telemetry providers and structured diagnostics. |

---

## 2.4 Delivery Layer

The delivery layer exposes the system through concrete interfaces such as HTTP or command-line
execution. Delivery projects should stay thin and delegate business behavior inward.

### 2.4.1 API Host

> HTTP-facing access to application use cases.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Minimal host bootstrap | 🟢 | The API project starts, configures DI, and exposes a basic health endpoint. | Replace the placeholder endpoint with real use-case-driven endpoints. |
| Health endpoint | 🟢 | A simple health check endpoint is available. | Add deeper readiness or dependency health checks if required. |
| Endpoint composition | 🟡 | The host is ready to map additional endpoints. | Add route groups, versioning, request models, and response contracts. |
| Authentication and authorization | 🔴 | Secure APIs usually need authentication and policy enforcement. | Choose the security model and integrate it consistently. |
| OpenAPI / API documentation | 🔴 | APIs benefit from discoverability and contract visibility. | Add OpenAPI generation and document the public surface. |

### 2.4.2 CLI Host

> Command-line access for automation, maintenance, or local development tasks.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Console host bootstrap | 🟢 | The CLI project runs and resolves application and infrastructure services. | Replace placeholder output with real command dispatch. |
| Command execution model | 🟡 | The project is ready for arguments and command routing. | Add explicit commands, argument parsing, and exit-code conventions. |
| Administrative workflows | 🔴 | CLI tools often run operational or data-related tasks. | Add maintenance, import/export, migration, or diagnostic commands. |
| Batch processing | 🔴 | CLI hosts can support scheduled or long-running jobs. | Define workflows and resource handling for non-interactive execution. |

### 2.4.3 Additional Delivery Options

> Potential future entry points.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Background worker host | 🔴 | Some systems benefit from a dedicated worker process. | Add a worker project when asynchronous or scheduled workloads appear. |
| Additional transports | 🔴 | gRPC, messaging consumers, or UI applications may be needed later. | Create separate hosts instead of overloading API or CLI responsibilities. |

---

## 2.5 Quality and Operations

This section captures repository-wide capabilities that support development, testing, and delivery.

### 2.5.1 Testing

> Fast feedback and confidence in the core logic.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Domain test project | 🟢 | A starter xUnit project exists for domain-focused tests. | Replace placeholder tests with actual business behavior tests. |
| Application test project | 🟢 | A starter xUnit project exists for application-focused tests. | Add use-case tests, validation tests, and edge-case coverage. |
| Integration tests | 🔴 | Infrastructure and delivery boundaries often need broader validation. | Add dedicated integration test projects when real adapters exist. |
| End-to-end verification | 🔴 | Complex systems often need full-path runtime verification. | Add E2E tests only when the product surface justifies them. |

### 2.5.2 Build and Delivery Tooling

> Shared tooling that supports consistent execution across environments.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| Central package management | 🟢 | `Directory.Packages.props` manages package versions centrally. | Keep versions current and remove packages that become unnecessary. |
| Shared build configuration | 🟢 | `Directory.Build.props` defines repository-wide defaults. | Tighten warnings, analyzers, and quality gates as the codebase matures. |
| GitHub Actions CI | 🟢 | Pull requests and pushes to key branches trigger build, test, and Docker validation. | Extend caching, reports, or release automation when needed. |
| SDK pinning | 🟢 | `global.json` locks the expected .NET SDK. | Update intentionally and keep CI aligned with the chosen SDK version. |

### 2.5.3 Containers and Runtime Operations

> Operational packaging for local development and deployment.

| Feature | Status | Description | What is missing |
|---|---|---|---|
| API Dockerfile | 🟢 | The API project can be built into a container image. | Add environment-specific runtime hardening if deployment requires it. |
| CLI Dockerfile | 🟢 | The CLI project can be built into a container image. | Add command scenarios, entrypoint variants, or job-specific images if needed. |
| Docker Compose orchestration | 🟢 | Local multi-container startup is supported. | Add health checks, environment files, volumes, and network rules as needed. |
| Observability and runtime diagnostics | 🔴 | Production systems need logs, metrics, traces, and dashboards. | Define a telemetry stack and deployment-facing monitoring strategy. |

---

## 3. Dependency Rules

The generated solution starts with the following references:

```text
FlowForge.Application       -> FlowForge.Domain
FlowForge.Infrastructure    -> FlowForge.Application, FlowForge.Domain
FlowForge.Api               -> FlowForge.Application, FlowForge.Infrastructure
FlowForge.CLI               -> FlowForge.Domain, FlowForge.Application, FlowForge.Infrastructure
FlowForge.Domain.Tests      -> FlowForge.Domain
FlowForge.Application.Tests -> FlowForge.Application
```

Rules to preserve:

- `Domain` must stay independent
- `Application` must not depend on `Infrastructure`, `Api`, or `CLI`
- `Infrastructure` should implement contracts, not define application policy
- delivery projects may compose services, but should not become logic-heavy
- tests may reference the layer they validate and required support libraries

---

## 4. Request and Execution Flows

### API flow

```text
HTTP Request
  -> API endpoint
  -> Application use case
  -> Domain logic
  -> Infrastructure implementation (if needed)
  -> Response DTO / result
```

### CLI flow

```text
CLI command
  -> argument parsing / command dispatch
  -> Application use case
  -> Domain logic
  -> Infrastructure implementation (if needed)
  -> console output / exit code
```

---

## 5. Repository Structure

```text
.
|-- AGENTS.md
|-- FlowForge.slnx
|-- Directory.Build.props
|-- Directory.Packages.props
|-- global.json
|-- src/
|   |-- FlowForge.Domain/
|   |-- FlowForge.Application/
|   |-- FlowForge.Infrastructure/
|   |-- FlowForge.Api/
|   `-- FlowForge.CLI/
|-- tests/
|   |-- FlowForge.Domain.Tests/
|   `-- FlowForge.Application.Tests/
|-- docs/
|   |-- Architecture.md
|   |-- Roadmap.md
|   `-- Todo.md
`-- .github/workflows/ci.yml
```

---

## 6. Architecture Decision Log

Use this section as a lightweight place to record major changes.

| Date | Decision | Reason |
|---|---|---|
| YYYY-MM-DD | Initial Clean Architecture bootstrap | Establish a modular, testable baseline |

Add new rows whenever the architecture changes in a meaningful way.
