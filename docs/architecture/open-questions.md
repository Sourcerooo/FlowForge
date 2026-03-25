# Open Architecture Questions

- Should snapshot publication default to fixed intervals, event-triggered updates, or a hybrid model?
- Which DTOs should live in shared contracts versus staying delivery-specific?
- Should the desktop playback timeline keep all published snapshots for a run in memory for MVP, or already support spillover or compaction strategies?
- Which scenario and layout parts are guaranteed immutable enough to be shared by reference inside snapshots?
- Should `GenerateSimulationEvent` use fixed time windows, scenario-defined cadence, or a pluggable generation strategy?
- Which event payload fields should be mandatory versus derived from `SimulationState` during handling?
- How exactly should `ProcessingToken` behave when work is paused, re-queued, or later disrupted?
- Should event priorities be represented as numeric constants, an enum with explicit ordering, or a dedicated policy service?
- Do we want a dedicated event type for on-hold and resumed behavior, or should the first extension model those transitions through existing generic events plus reasons?
- Should disturbances such as outages and shipping stops enter directly after the MVP core, or only after scenario persistence and replay are stable?
- Should the event vocabulary also be renamed from `OrderQueued` and `OrderCompleted` to `WorkItemQueued` and `WorkItemCompleted`, or should only the internal runtime model become generic first?
- How much metadata must `TrackingSubjectReference` carry for the MVP beyond `EntityType` and `ExternalEntityId`?
- Do stage references inside events and snapshots use raw `Guid`, typed value objects wrapping `Guid`, or immutable configuration references?
- Should the scenario loader accept only the hierarchical object form for `stages` and `stations`, or also support array-based input for easier external tooling later?
