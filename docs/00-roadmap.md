# FlowForge – .NET Cloud Boot Camp Roadmap

> **Audience:** Senior / experienced software engineers  
> **Goal:** Become productive in modern .NET cloud development with Clean Architecture  
> **Project:** FlowForge – Supply Chain Planning Service

---

## 📌 Project Progress Tracker

| Phase | Description | Weight | Status |
|------|------------|--------|--------|
| Phase 1 | Modern C# & Async Foundations | 25% | ⬜ Not started |
| Phase 2 | Runtime, Packages & Serialization | 15% | ⬜ Not started |
| Phase 3 | ASP.NET Core & API Surface | 20% | ⬜ Not started |
| Phase 4 | Persistence & Reliability | 20% | ⬜ Not started |
| Phase 5 | Background Processing & Performance | 15% | ⬜ Not started |
| Phase 6 | Cloud Readiness & Testing | 5% | ⬜ Not started |
| **Total** |  | **100%** | **0% complete** |

> Each day updates the cumulative percentage.  
> Easier topics intentionally contribute less weight.

---

## 📚 Table of Contents

- [FlowForge – .NET Cloud Boot Camp Roadmap](#flowforge--net-cloud-boot-camp-roadmap)
  - [📌 Project Progress Tracker](#-project-progress-tracker)
  - [📚 Table of Contents](#-table-of-contents)
- [Phase 1 – Modern C# \& Async Foundations](#phase-1--modern-c--async-foundations)
  - [Day 1 – Domain Modeling with Records](#day-1--domain-modeling-with-records)
    - [Progress](#progress)
    - [Focus](#focus)
    - [Tasks](#tasks)
  - [Day 2 – Pattern Matching \& Business Rules](#day-2--pattern-matching--business-rules)
    - [Progress](#progress-1)
    - [Focus](#focus-1)
    - [Tasks](#tasks-1)
  - [Day 3 – Nullable Reference Types](#day-3--nullable-reference-types)
    - [Progress](#progress-2)
    - [Focus](#focus-2)
    - [Tasks](#tasks-2)
  - [Day 4 – IDisposable and Resource Lifetime](#day-4--idisposable-and-resource-lifetime)
    - [Progress](#progress-3)
    - [Features](#features)
    - [Tasks](#tasks-3)
  - [Day 5 – async/await \& Task](#day-5--asyncawait--task)
    - [Progress](#progress-4)
    - [Focus](#focus-3)
    - [Tasks](#tasks-4)
  - [Day 6 – ValueTask \& Span](#day-6--valuetask--span)
    - [Progress](#progress-5)
    - [Focus](#focus-4)
    - [Tasks](#tasks-5)
- [Phase 2 – Runtime, Packages \& Serialization](#phase-2--runtime-packages--serialization)
  - [Day 7 – Assemblies \& NuGet](#day-7--assemblies--nuget)
    - [Progress](#progress-6)
    - [Focus](#focus-5)
    - [Tasks](#tasks-6)
  - [Day 8 – JSON \& Serialization Boundaries](#day-8--json--serialization-boundaries)
    - [Progress](#progress-7)
    - [Focus](#focus-6)
    - [Tasks](#tasks-7)
  - [Day 9 – Dependency Injection](#day-9--dependency-injection)
    - [Progress](#progress-8)
    - [Focus](#focus-7)
    - [Tasks](#tasks-8)
  - [Day 10 – Configuration \& Options pattern](#day-10--configuration--options-pattern)
    - [Progress](#progress-9)
    - [Focus](#focus-8)
    - [Tasks](#tasks-9)
  - [Day 11 – Logging \& Obvervability](#day-11--logging--obvervability)
    - [Progress](#progress-10)
    - [Focus](#focus-9)
    - [Tasks](#tasks-10)
- [Phase 3 – ASP.NET Core \& APIs](#phase-3--aspnet-core--apis)
  - [Day 12 – API Surface Design](#day-12--api-surface-design)
    - [Progress](#progress-11)
    - [Focus](#focus-10)
    - [Tasks](#tasks-11)
  - [Day 13 – Middleware \& Error Handling](#day-13--middleware--error-handling)
    - [Progress](#progress-12)
    - [Focus](#focus-11)
    - [Tasks](#tasks-12)
  - [Day 14 – Validation \& Semantics](#day-14--validation--semantics)
    - [Progress](#progress-13)
    - [Focus](#focus-12)
    - [Tasks](#tasks-13)
  - [Day 15 – Swagger / OpenAPI](#day-15--swagger--openapi)
    - [Progress](#progress-14)
    - [Focus](#focus-13)
    - [Tasks](#tasks-14)
- [Phase 4 – Persistence \& Reliability](#phase-4--persistence--reliability)
  - [Day 16 – EF Core Fundamentals](#day-16--ef-core-fundamentals)
    - [Progress](#progress-15)
    - [Focus](#focus-14)
    - [Tasks](#tasks-15)
  - [Day 17 – Transactions \& Consistency](#day-17--transactions--consistency)
    - [Progress](#progress-16)
    - [Focus](#focus-15)
    - [Tasks](#tasks-16)
  - [Day 18 – Connection Pooling \& Lifetimes](#day-18--connection-pooling--lifetimes)
    - [Progress](#progress-17)
    - [Focus](#focus-16)
    - [Tasks](#tasks-17)
  - [Day 19 – Dapper for Hot Paths](#day-19--dapper-for-hot-paths)
    - [Progress](#progress-18)
    - [Focus](#focus-17)
    - [Tasks](#tasks-18)
- [Phase 5 – Background Processing \& Performance](#phase-5--background-processing--performance)
  - [Day 20 – Background Workers](#day-20--background-workers)
    - [Progress](#progress-19)
    - [Focus](#focus-18)
    - [Tasks](#tasks-19)
  - [Day 21 – Channels \& Throughput](#day-21--channels--throughput)
    - [Progress](#progress-20)
    - [Focus](#focus-19)
    - [Tasks](#tasks-20)
- [Phase 6 – Cloud Readiness \& Testing](#phase-6--cloud-readiness--testing)
  - [Day 22 – Docker, Health \& Testing](#day-22--docker-health--testing)
    - [Progress](#progress-21)
    - [Focus](#focus-20)
    - [Tasks](#tasks-21)
- [Phase 7 - Testing](#phase-7---testing)
  - [Day 23 – Unit \& Integration Tests](#day-23--unit--integration-tests)
    - [Progress](#progress-22)
    - [Focus](#focus-21)
    - [Tasks](#tasks-22)
  - [✅ Completion Criteria](#-completion-criteria)

---

# Phase 1 – Modern C# & Async Foundations
**Weight:** 25%

---

## Day 1 – Domain Modeling with Records
### Progress
▓░░░░░░░░░░░░░░░░░░░░░░░░ 5%

### Focus
- `record`, immutability
- `init`
- Value vs reference semantics
- Domain purity

### Tasks
- Create Domain project
- Define immutable domain models:
  - Product
  - Location
  - InventorySnapshot
  - Demand
- No framework references allowed

---

## Day 2 – Pattern Matching & Business Rules
### Progress
▓░░░░░░░░░░░░░░░░░░░░░░░░ 7%

### Focus
- `switch` expressions
- Property & type patterns
- Guard clauses

### Tasks
- Implement PlanningIssue generation using pattern matching
- Replace conditional logic with declarative rule expressions

---

## Day 3 – Nullable Reference Types
### Progress
▓▓░░░░░░░░░░░░░░░░░░░░░░░ 10%

### Focus
- Nullable annotations
- Compiler enforcement
- Boundary safety
- Defensive API design

### Tasks
- Enable nullable reference types in all projects
- Define explicit nullability in DTOs
- Fix warnings intentionally
- Decide what *must* be non-null

---

## Day 4 – IDisposable and Resource Lifetime
### Progress
▓▓▓░░░░░░░░░░░░░░░░░░░░░░ 13%

### Features
- `IDisposable`
- `using`
- Deterministic cleanup

### Tasks
Write a small repository abstraction that:
- Opens a DB connection
- Logs lifecycle
- Cleans up properly
  
---

## Day 5 – async/await & Task
### Progress
▓▓▓▓░░░░░░░░░░░░░░░░░░░░░ 20%

### Focus
- async I/O
- CancellationToken propagation
- Task-based APIs

### Tasks
- Make all application use cases async
- Propagate CancellationToken from API → Infra
- Remove all blocking calls

---

## Day 6 – ValueTask & Span<T>
### Progress
▓▓▓▓▓░░░░░░░░░░░░░░░░░░░░ 25%

### Focus
- Allocation-aware async APIs
- `Span<T>` / `ReadOnlySpan<T>` parsing

### Tasks
- Implement CSV import using spans
- Introduce ValueTask in cached lookups
- Document when ValueTask is *not* appropriate

---

# Phase 2 – Runtime, Packages & Serialization
**Weight:** 15%

---

## Day 7 – Assemblies & NuGet
### Progress
▓▓▓▓▓░░░░░░░░░░░░░░░░░░░░ 27%

### Focus
- Project boundaries
- NuGet versioning
- Dependency hygiene

### Tasks
- Split solution into:
  - Domain
  - Application
  - Infrastructure
  - API
- Introduce central package management

---

## Day 8 – JSON & Serialization Boundaries
### Progress
▓▓▓▓▓▓░░░░░░░░░░░░░░░░░░░ 32%

### Focus
- System.Text.Json
- DTO ↔ Domain separation
- Serialization options

### Tasks
- Configure JSON globally
- Define API DTOs
- Add converters where needed

---

## Day 9 – Dependency Injection
### Progress
▓▓▓▓▓▓▓░░░░░░░░░░░░░░░░░░ 36%

### Focus
- DI lifetimes (scoped/singleton/transient)
- Options pattern
- Constructor injection

### Tasks
- Register all services via DI
- Register `IInventoryService`
- Add PlanningOptions
- Add correlation-aware logging

---

## Day 10 – Configuration & Options pattern
### Progress
▓▓▓▓▓▓▓░░░░░░░░░░░░░░░░░░ 38%

### Focus
- `appsettings.json`
- Environment variables
- `IOptions<T>

### Tasks
- Create `PlanningOptions`
- Configure max planning horizon
- Read it inside the planner

## Day 11 – Logging & Obvervability
### Progress
▓▓▓▓▓▓▓▓░░░░░░░░░░░░░░░░░ 40%

### Focus
- `ILogger<T>`
- Structured Logging
- Log levels

### Tasks
- Log each planning run
- Include correlation ID
- Log structured data, no strings

--- 

# Phase 3 – ASP.NET Core & APIs
**Weight:** 20%

---

## Day 12 – API Surface Design
### Progress
▓▓▓▓▓▓▓▓▓░░░░░░░░░░░░░░░░ 48%

### Focus
- Minimal APIs
- Controlles
- Routing
- HTTP semantics
- Async endpoints

### Tasks
- Implement:
  - GET /products
  - GET /locations
  - GET /inventory
  - POST /planning-runs
  - GET /planning-runs
  - GET /planning-runs/{id}
  - POST /imports/demand

---

## Day 13 – Middleware & Error Handling
### Progress
▓▓▓▓▓▓▓▓▓▓░░░░░░░░░░░░░░░ 50%

### Focus
- Middleware pipeline
- Problem Details
- Global exception handling

### Tasks
- Add global exception middleware
- Map domain errors to HTTP responses

---

## Day 14 – Validation & Semantics
### Progress
▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░░░░░░░ 55%

### Focus
- Model validation
- Proper HTTP status codes
- Idempotency

### Tasks
- Validate planning requests
- Return
  - 400 for bad input
  - 404 for missing entities
  - 202 for async planning


--- 

## Day 15 – Swagger / OpenAPI
### Progress
▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░░░░░░ 60%

### Focus
- OpenAPI contracts
- API discoverability

### Tasks
- Enable Swagger UI
- Document request/response models
- Include error responses

---

# Phase 4 – Persistence & Reliability
**Weight:** 20%

---

## Day 16 – EF Core Fundamentals
### Progress
▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░░░░░ 65%

### Focus
- DbContext
- Migrations
- Tracking vs no-tracking

### Tasks
- Persist core entities
- Create initial migrations

---

## Day 17 – Transactions & Consistency
### Progress
▓▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░░░░ 70%

### Focus
- Transaction boundaries
- Atomic operations

### Tasks
- Make planning-run creation transactional
- Ensure idempotent behavior

---

## Day 18 – Connection Pooling & Lifetimes
### Progress
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░░░ 75%

### Focus
- ADO.NET pooling
- DbContext lifetimes

### Tasks
- Configure DbContext correctly
- Observe connection usage under load

---

## Day 19 – Dapper for Hot Paths
### Progress
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░░ 80%

### Focus
- Dapper
- Raw SQL
- Performance-critical reads

### Tasks
- Implement shortage report using Dapper

---

# Phase 5 – Background Processing & Performance
**Weight:** 15%

---

## Day 20 – Background Workers
### Progress
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░ 88%

### Focus
- IHostedService
- Task scheduling
- Thread pool behavior
- Async pitfalls
- Long-running jobs

### Tasks
- Execute planning asynchronously
- API returns immediately (202)

---

## Day 21 – Channels & Throughput
### Progress
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░ 95%

### Focus
- `System.Threading.Channels`
- Producer/consumer
- Backpressure

### Tasks
- Queue planning runs via Channels
- Limit concurrency explicitly

---

# Phase 6 – Cloud Readiness & Testing
**Weight:** 5%

---

## Day 22 – Docker, Health & Testing
### Progress
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░ 97%

### Focus
- Docker
- Health checks
- Integration testing

### Tasks
- Dockerize FlowForge
- Add liveness/readiness endpoints
- Write end-to-end API tests
- Add `/health`endpoint

---

# Phase 7 - Testing

## Day 23 – Unit & Integration Tests
### Progress
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ 100%

### Focus
- xUnit
- Mocking (moq)
- WebApplicationFactory

### Tasks
- Test planning logic
- Test full API flow

---

## ✅ Completion Criteria

- Clean Architecture respected
- Async end-to-end
- Swagger-documented API
- Background processing
- Transaction-safe persistence
- Cloud-ready deployment

