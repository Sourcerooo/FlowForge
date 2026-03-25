# Architecture Documentation Index

This index describes how the architecture documentation is organized and which document should be used for which question.

## Reading Order

Use this reading order unless a task is scoped to one specific component.

1. `docs/Architecture.md`
2. `docs/architecture/glossary.md`
3. Relevant file in `docs/architecture/components/`
4. Relevant file in `docs/architecture/design/`
5. Relevant file in `docs/architecture/decisions/`
6. Optional examples in `docs/architecture/examples/`

## Which Document To Use

| Question | Read |
|---|---|
| What is the product and high-level architecture? | `docs/Architecture.md` |
| What does a component own and what must it not own? | `docs/architecture/components/*.md` |
| Which interfaces, DTOs, runtime structures, or data flows are expected? | `docs/architecture/design/*.md` |
| Is something an example or a binding rule? | Check whether the file is in `examples/` or another directory |
| Why was a design direction chosen? | `docs/architecture/decisions/*.md` |
| Which terms are canonical? | `docs/architecture/glossary.md` |
| Which questions are still unresolved? | `docs/architecture/open-questions.md` |
| What is already implemented and what is still immature? | `docs/architecture/current-state.md` |

## Directory Map

```text
docs/
  Architecture.md
  architecture/
    index.md
    glossary.md
    current-state.md
    open-questions.md
    components/
      application.md
      delivery.md
      domain.md
      infrastructure.md
      quality-and-operations.md
      simulation.md
    design/
      application-contracts.md
      checkpoints.md
      scenario-configuration.md
      simulation-dispatching.md
      simulation-events.md
      simulation-execution-context.md
      simulation-orchestration.md
      simulation-runner.md
      simulation-runtime.md
      snapshots-and-kpis.md
    examples/
      checkpoint-flow.md
      simulation-lifecycle.md
    decisions/
      2026-03-17-mvp-and-delivery.md
      2026-03-17-simulation-and-snapshots.md
      2026-03-19-runtime-model-and-configuration.md
      2026-03-20-process-orchestration.md
      2026-03-25-in-memory-runtime-and-context-di.md
```

## Document Rules

- `Architecture.md` stays short and stable.
- `components/` defines responsibilities and boundaries, not exact interface signatures.
- `design/` defines technical contracts, interface direction, state ownership, and data flow.
- `examples/` is informative only and must not be treated as the sole normative source.
- `decisions/` records architecture decisions that are currently in force.
- If a topic grows too large, split it into additional component- or design-specific files instead of expanding `Architecture.md`.

## Authoring Guidance

- Put high-level changes into `docs/Architecture.md`.
- Put component responsibility changes into `docs/architecture/components/`.
- Put interface, DTO, event, checkpoint, and runtime-flow changes into `docs/architecture/design/`.
- Split large design topics into narrower files such as events, dispatching, runtime loop, execution context, and orchestration instead of growing one broad design document.
- Put sample implementations, pseudocode, and illustrative flows into `docs/architecture/examples/`.
- Put confirmed architecture decisions into a dated file under `docs/architecture/decisions/`.
- Put unresolved questions into `docs/architecture/open-questions.md`.
