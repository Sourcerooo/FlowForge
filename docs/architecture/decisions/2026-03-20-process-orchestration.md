# 2026-03-20 -- Process Orchestration Boundary

## Status

Accepted

## Decision

Introduce `IWorkItemProcessOrchestrator` as the simulation-side coordination boundary for event-driven process steps, while keeping `WorkItemTracking`, `StationTracking`, and `StageTracking` as behavior-rich mutable runtime models.

## Reasoning

- Queueing, starting, completion, routing, KPI updates, and follow-up scheduling belong to one use case.
- The detailed mutations still belong to the runtime objects that own their data and invariants.
- This avoids both an anemic model and a giant service that edits every field directly.
