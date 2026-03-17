# Roadmap -- FlowForge

## Purpose

This roadmap captures the planned evolution of the project at a milestone level.
It is intentionally broader than the task list in `docs/Todo.md` and should answer the question:
"What capabilities do we want to have next, and in which order?"

## Planning Principles

- prioritize business value over technical novelty
- keep the dependency direction intact while adding features
- deliver in thin vertical slices where possible
- document major architectural changes before implementation starts
- revise the roadmap regularly as the product scope changes

## Milestones

### Milestone 1 -- Foundation

Goal: turn the generated scaffold into a usable working baseline.

- define the first domain concepts and invariants
- implement the first application use case
- connect dependency injection registrations
- decide on configuration, logging, and error handling basics
- replace placeholder entry-point behavior in API and CLI

### Milestone 2 -- Persistence and Integrations

Goal: connect the application to real external systems.

- choose a persistence approach
- implement repositories or data access services
- add external provider abstractions where needed
- create initial integration tests
- document infrastructure boundaries and operational assumptions

### Milestone 3 -- Delivery Workflows

Goal: expose stable user or system-facing workflows.

- expand API endpoints around meaningful use cases
- add CLI commands for administration, automation, or maintenance
- define request and response contracts
- add validation, error mapping, and consistent result handling
- review observability and operational diagnostics

### Milestone 4 -- Quality and Automation

Goal: improve confidence, repeatability, and maintainability.

- broaden test coverage for domain and application logic
- add integration and end-to-end checks where valuable
- harden CI and container workflows
- establish release and versioning conventions
- add deployment-specific documentation if required

### Milestone 5 -- Product Expansion

Goal: prepare the solution for sustained growth.

- split modules further if responsibilities become too broad
- add background processing or additional delivery projects when needed
- review performance and scalability bottlenecks
- introduce architecture decision records for major technical choices
- refine roadmap and backlog around real product feedback

## Open Questions

Use this section to capture strategic uncertainties before they become implementation blockers.

- Which persistence technology best fits the product requirements?
- Which external systems must be integrated first?
- Does the CLI act mainly as a developer tool, an admin tool, or a batch-processing host?
- Which deployment environments need to be supported initially?
- What non-functional requirements are most critical: latency, throughput, traceability, or cost?

## Revision Log

| Date | Change |
|---|---|
| YYYY-MM-DD | Initial roadmap created from the project template |
