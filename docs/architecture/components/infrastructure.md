# Infrastructure Component

## Purpose

`FlowForge.Infrastructure` implements the technical concerns required by the application and delivery hosts without becoming the owner of business rules.

## Responsibilities

- load and persist scenarios
- persist checkpoints and other technical documents
- implement export and integration adapters
- perform JSON mapping and normalization into domain or simulation-owned models
- host operational support concerns such as diagnostics plumbing where needed

## Owns

- file, JSON, and later database access
- infrastructure-side mapping and adapter implementations
- environment-specific technical wiring

## Does Not Own

- business invariants
- canonical domain process topology
- simulation runtime behavior
- API or desktop product policy

## Boundary Rules

- infrastructure implements ports defined inward, usually in `FlowForge.Application`
- infrastructure may deserialize raw persistence documents before mapping them into domain-owned or simulation-owned contracts
- infrastructure must not become a second source of truth for process semantics
