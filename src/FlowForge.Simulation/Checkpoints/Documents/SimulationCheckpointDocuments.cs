using System.Text.Json.Nodes;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Checkpoints.Documents;

public sealed record SimulationStateDocument(
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

public sealed record SimulationRunMetadataDocument(
    Guid SimulationRunId,
    string ScenarioKey,
    string EngineVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastSavedAtUtc,
    string? CreatedBy,
    IReadOnlyDictionary<string, string>? Tags);

public sealed record ProcessConfigurationDocument(
    string ScenarioKey,
    string Name,
    TimeSpan PlannedDuration,
    ArrivalProfileDocument ArrivalProfile,
    IReadOnlyList<StageDefinitionDocument> Stages);

public sealed record ArrivalProfileDocument(
    TimeSpan GenerationWindow,
    int AverageWorkItemsPerWindow,
    int? MaxWorkItemsPerWindow);

public sealed record StageDefinitionDocument(
    Guid StageId,
    string StageKey,
    string DisplayName,
    int Sequence,
    IReadOnlyList<StationDefinitionDocument> Stations);

public sealed record StationDefinitionDocument(
    Guid StationId,
    Guid StageId,
    string StationKey,
    string DisplayName,
    int WorkerCount,
    TimeSpan AverageProcessingTime);

public sealed record SimulationRunOptionsDocument(
    bool AutoStart,
    bool PublishSnapshots,
    TimeSpan? SnapshotInterval,
    bool RetainSnapshotTimeline,
    string? Notes,
    IReadOnlyDictionary<string, string>? Parameters);

public sealed record SimulationRuntimeStateDocument(
    TimeSpan CurrentTime,
    string Status,
    long NextSequenceNumber,
    long WorkItemsCreated,
    long WorkItemsCompleted,
    long WorkItemsInProgress,
    IReadOnlyDictionary<string, JsonNode?>? StateBag);

public sealed record SimulationEventDocument(
    string EventType,
    Guid EventId,
    Guid SimulationRunId,
    TimeSpan ScheduledTime,
    string EventKind,
    string ProcessStage,
    int SortRank,
    long SequenceNumber,
    ProcessingToken? ProcessingToken,
    Guid? OrderId,
    IReadOnlyDictionary<string, JsonNode?>? Payload);

public sealed record TrackingStateDocument(
    IReadOnlyList<WorkItemTrackingDocument> WorkItems,
    IReadOnlyList<StationTrackingDocument> Stations,
    IReadOnlyDictionary<string, JsonNode?>? TrackingBag);

public sealed record WorkItemTrackingDocument(
    Guid TrackingSubjectId,
    Guid ExternalEntityId,
    string EntityType,
    string CurrentStatus,
    Guid? CurrentStageId,
    TimeSpan CreatedAt,
    TimeSpan? CompletedAt,
    ProcessingToken CurrentProcessingToken,
    IReadOnlyList<WorkItemTrackingSegmentDocument> Segments);

public sealed record WorkItemTrackingSegmentDocument(
    long SegmentId,
    ProcessingToken ProcessingToken,
    Guid? StageId,
    Guid? StationId,
    string SegmentType,
    TimeSpan StartedAt,
    TimeSpan? EndedAt,
    string? Reason);

public sealed record StationTrackingDocument(
    Guid StationId,
    Guid StageId,
    long WorkItemsQueuedCount,
    long WorkItemsStartedCount,
    long WorkItemsCompletedCount,
    long WorkItemsPlacedOnHoldCount,
    long WorkItemsRequeuedCount,
    TimeSpan CumulativeQueueWait,
    TimeSpan CumulativeProcessingTime,
    TimeSpan CumulativeOnHoldTime,
    TimeSpan CumulativeBusyTime,
    int PeakQueueLength,
    int PeakBusyWorkers);

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

public sealed record SnapshotDocument(
    long SnapshotSequence,
    TimeSpan SimulationTime,
    string Status,
    JsonObject Data);
