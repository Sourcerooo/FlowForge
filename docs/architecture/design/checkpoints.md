# Checkpoint Design

This document is normative for checkpoint-oriented save and load flows once checkpoint persistence is introduced.

## Boundary Rule

- `SimulationExecutionState` is a technical contract used for checkpoint save and load orchestration
- live execution collaborators such as queue adapters, dispatcher, and scheduler remain simulation-internal runtime concerns
- `Application` decides when a checkpoint is saved or restored
- `Infrastructure` implements file or later database persistence
- checkpoint document models do not belong in `Domain`

## First Checkpoint Interfaces

```csharp
public interface ISimulationCheckpointStore
{
    Task SaveAsync(
        SimulationExecutionState state,
        string filePath,
        CancellationToken cancellationToken = default);

    Task<SimulationExecutionState> LoadAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}

public interface ISimulationCheckpointBuilder
{
    SimulationCheckpointDocument Build(SimulationExecutionState state);
}

public interface ISimulationStateBuilder
{
    SimulationExecutionState Build(SimulationCheckpointDocument checkpoint);
}
```

Recommended ownership split:

- `ISimulationCheckpointStore` lives in `FlowForge.Application` and is implemented by `FlowForge.Infrastructure`
- `SimulationExecutionState`, `SimulationCheckpointDocument`, `ISimulationCheckpointBuilder`, and `ISimulationStateBuilder` live in `FlowForge.Simulation`

## Save and Load Responsibility Split

- simulation maps between live execution state and `SimulationExecutionState`
- checkpoint builder transforms `SimulationExecutionState` into a serializable `SimulationCheckpointDocument`
- checkpoint store persists one portable JSON file such as `*.flowforge-run.json`
- state builder reconstructs `SimulationExecutionState` from the stored document

## File Format Direction

- use one single JSON file for portability and sharing
- keep logical sections inside the document instead of splitting the save across multiple files
- store process configuration and run options inside the checkpoint so the run is reproducible
- version the first format from the beginning

## Baseline Document Shape

```csharp
public sealed record SimulationExecutionState(
    Guid SimulationRunId,
    SimulationRunMetadataDocument RunMetadata,
    ProcessConfigurationDocument ProcessConfiguration,
    SimulationRunOptionsDocument RunOptions,
    SimulationRuntimeStateDocument RuntimeState,
    IReadOnlyList<SimulationEventDocument> EventQueue,
    TrackingStateDocument Tracking,
    KpiStateDocument KpiState,
    SnapshotStateDocument SnapshotState);

public sealed record SimulationCheckpointDocument(
    int FormatVersion,
    SimulationRunMetadataDocument RunMetadata,
    ProcessConfigurationDocument ProcessConfiguration,
    SimulationRunOptionsDocument RunOptions,
    SimulationRuntimeStateDocument RuntimeState,
    IReadOnlyList<SimulationEventDocument> EventQueue,
    TrackingStateDocument Tracking,
    KpiStateDocument KpiState,
    SnapshotStateDocument SnapshotState);
```

Recommended meaning:

- `SimulationExecutionState` is the technical in-memory transfer model for checkpoint-oriented flows
- `SimulationCheckpointDocument` is the exact serializable persistence shape
- the document contains everything required for reproducible resume and sharing

## Important Checkpoint Subdocuments

The checkpoint root should keep its major sections explicit so save files remain inspectable and versionable.

Recommended subdocument direction:

```csharp
public sealed record SimulationRunMetadataDocument(
    Guid SimulationRunId,
    string ScenarioKey,
    string EngineVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastSavedAtUtc,
    string? CreatedBy,
    IReadOnlyDictionary<string, string>? Tags);

public sealed record SimulationRunOptionsDocument(
    bool AutoStart,
    bool PublishSnapshots,
    TimeSpan? SnapshotInterval,
    bool RetainSnapshotTimeline,
    string? Notes,
    IReadOnlyDictionary<string, string>? Parameters);

public sealed record TrackingStateDocument(
    IReadOnlyList<WorkItemTrackingDocument> WorkItems,
    IReadOnlyList<StationTrackingDocument> Stations,
    IReadOnlyDictionary<string, JsonNode?>? TrackingBag);

public sealed record KpiStateDocument(
    long WorkItemsCreated,
    long WorkItemsCompleted,
    TimeSpan SumCompletedLeadTimes,
    TimeSpan? MinLeadTime,
    TimeSpan? MaxLeadTime,
    int CurrentWorkInProgress,
    int PeakWorkInProgress,
    IReadOnlyDictionary<string, JsonNode?>? Aggregates);

public sealed record SnapshotStateDocument(
    SnapshotDocument? LatestSnapshot,
    IReadOnlyList<SnapshotDocument> Timeline,
    IReadOnlyDictionary<string, JsonNode?>? SnapshotBag);
```

Recommended section intent:

- metadata captures run identity, provenance, and save timestamps
- run options capture the execution and publication settings needed for reproducible resume
- tracking state stores the factual work-item and station history required by the resumed runtime
- KPI state stores compact aggregates so resume does not require expensive recomputation
- snapshot state keeps latest and timeline data when retention is enabled
