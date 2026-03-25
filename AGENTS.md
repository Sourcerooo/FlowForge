# Agent Guidelines -- FlowForge

Preferences and working rules for AI coding agents operating in this repository.

---

## Language

- Chat responses should be in German unless explicitly requested otherwise.
- Code comments should be written in English.
- Repository documentation should be written in English.
- `docs/Todo.md` may remain German if the team uses it as an operational planning document.
- Use clear technical wording and avoid decorative language.

---

## Architecture Rules

- Respect Clean Architecture boundaries.
- Preserve dependency direction: `Domain <- Application <- Infrastructure <- Delivery`.
- Do not introduce references from `Domain` to outer layers.
- Keep business rules in `Domain` and use-case orchestration in `Application`.
- Keep `Api` and `CLI` thin; they are delivery hosts, not business-logic containers.
- Prefer extending existing modules over creating new projects unless a new boundary is justified.

---

## C# and .NET Guidelines

- Target the SDK and framework versions defined by `global.json` and the generated solution.
- Use nullable reference types correctly; do not silence warnings without reason.
- Prefer constructor injection for dependencies.
- Use explicit, intention-revealing type names for domain concepts.
- Keep classes focused and avoid god services.
- Prefer async APIs for I/O-bound operations; avoid fake async wrappers.
- Use `CancellationToken` for cancellable async flows when appropriate.
- Avoid static mutable state unless there is a strong architectural reason.
- Prefer composition over inheritance unless inheritance materially improves the model.

---

## Project Structure Expectations

### `FlowForge.Domain`

- Contains entities, value objects, domain services, and business rules.
- Must not depend on persistence, HTTP, UI, or infrastructure frameworks.

### `FlowForge.Application`

- Contains use cases, interfaces, DTOs, validation, and orchestration.
- May depend on `Domain`, but not on delivery projects.

### `FlowForge.Infrastructure`

- Contains implementations for storage, external services, and technical adapters.
- Should implement contracts defined in `Application`.

### `FlowForge.Api`

- Contains endpoint wiring and transport-specific configuration.
- Should delegate real behavior to `Application` services.

### `FlowForge.CLI`

- Contains command-line bootstrapping and command execution.
- Should reuse `Application` use cases instead of duplicating logic.

---

## Testing Expectations

- Add or update tests for meaningful behavior changes.
- Prefer unit tests close to the owning layer.
- Add integration tests when behavior crosses infrastructure boundaries.
- Do not leave placeholder tests in place when implementing real features.
- Keep test names descriptive and behavior-oriented.

---

## Documentation Expectations

- Update `README.md` when setup, execution, or repository structure changes.
- Keep architecture documentation split by level:
  - `docs/Architecture.md` for tech stack, high-level architecture, components, dependencies, and core rules only
  - `docs/architecture/components/*.md` for component responsibilities and boundaries
  - `docs/architecture/design/*.md` for interfaces, technical contracts, state ownership, and data flows
  - `docs/architecture/examples/*.md` for non-normative examples and reference implementations
  - `docs/architecture/decisions/*.md` for accepted architecture decisions
  - `docs/architecture/glossary.md` for canonical terminology
- Update `docs/Architecture.md` when layer responsibilities, component boundaries, dependency direction, or the high-level tech stack changes.
- Update the matching file in `docs/architecture/components/` when a component gains or changes responsibilities.
- Update the matching file in `docs/architecture/design/` when interfaces, DTOs, runtime flows, checkpoint models, or implementation-facing design decisions change.
- Update `docs/architecture/examples/` only for illustrative material; do not store binding rules there.
- Add new accepted architecture decisions to `docs/architecture/decisions/` instead of extending one large decision log.
- Update `docs/Roadmap.md` for milestone-level planning changes.
- Update `docs/Todo.md` for concrete next tasks and operational follow-up only.
- Use `docs/DoneTasks.md` for tasks that are already completed.
- When a task is completed, remove it from `docs/Todo.md` and move it to `docs/DoneTasks.md` in the same work session.
- Update `docs/Vision.md` for longer-term work, future decisions, and non-immediate follow-up topics.
- Whenever an architecture decision is made, a new todo appears, or new features are identified, update the relevant documentation immediately in the same work session.
- Prefer documenting important decisions rather than relying on implicit repository knowledge.

---

## Implementation Guidelines for Agents

- Read the relevant files before changing them.
- Prefer editing existing files over creating new ones unless a new file is clearly needed.
- Follow the existing naming and structural conventions of the repository.
- Do not add dependencies lightly; mention new packages explicitly when they are required.
- Avoid speculative refactors unrelated to the requested task.
- Keep generated code and templates maintainable and easy to inspect.
- If a task fails partway through, leave the repository in a clean and understandable state.

---

## Git and Change Hygiene

- Do not revert user changes you did not create.
- Keep commits focused when commits are requested.
- Avoid destructive Git operations unless explicitly requested.
- Do not commit secrets, local credentials, or environment-specific files.

---

## Preferred Workflow

1. Inspect the affected files and surrounding architecture.
2. Make the smallest change that solves the requested problem cleanly.
3. Validate with relevant commands such as `dotnet build` or `dotnet test` when possible.
4. Summarize what changed, where it changed, and any follow-up work that remains.
