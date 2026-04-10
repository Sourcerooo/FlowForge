# Snapshots and KPI Design

This document is normative for immutable consumer-facing snapshots and KPI ownership.

## Publication Direction

Recommended publication flow:

```text
SimulationState + WorkItemTracking + StationTracking
  -> KPI collector / derived metrics
  -> Snapshot builder
  -> immutable SimulationSnapshot
  -> latest snapshot store and optional timeline store
  -> Desktop / API / replay consumers
```

Recommended default:

- KPI facts are maintained incrementally during runtime event handling
- the snapshot builder reads the current KPI aggregates and embeds them into the snapshot
- API, CLI, and desktop consumers read KPI values from the snapshot instead of recalculating them

## Ownership Model

| Component | Owns | Does not own |
|---|---|---|
| `SimulationState` | current mutable runtime state | published snapshots |
| `WorkItemTracking` | per-work-item timing history | snapshot retention |
| `StationTracking` | cumulative station facts | UI-facing DTOs |
| `IKpiCollector` | derived KPI aggregates | snapshot lifecycle |
| `ISnapshotBuilder` | immutable DTO construction | runtime mutation |
| `ISnapshotStore` | latest snapshot availability | mutable runtime state |
| `ISnapshotTimelineStore` | ordered playback history | KPI calculations |

## Snapshot Publication Semantics

`SnapshotPublishedEvent` acts as a publication trigger and should not carry the full snapshot payload.

Recommended handling flow:

1. runner dequeues `SnapshotPublishedEvent`
2. snapshot handler reads the current `SimulationState`
3. snapshot handler reads current KPI aggregates
4. snapshot builder creates a new immutable snapshot object graph
5. latest snapshot store atomically swaps the current snapshot reference
6. timeline store optionally retains the ordered snapshot

## Snapshot Root Contract

Recommended baseline shape:

```csharp
public sealed record SimulationSnapshot(
    Guid SimulationRunId,
    long SnapshotSequence,
    TimeSpan SimulationTime,
    SimulationStatus Status,
    ScenarioSnapshot Scenario,
    ProcessSnapshot Process,
    KpiSnapshot Kpis,
    AlertSnapshot Alerts,
    SnapshotMetadata Metadata);
```

Recommended interpretation:

- `SimulationRunId` ties the snapshot to one execution
- `SnapshotSequence` gives readers monotonic ordering independent of event sequence numbers
- `SimulationTime` is the business time visible to consumers
- `Scenario`, `Process`, and `Kpis` provide the read model needed by desktop, API, and replay readers

## Snapshot Data Structures

Recommended decomposition:

- `ScenarioSnapshot`: scenario parameters relevant for readers
- `ProcessSnapshot`: global process counters, station views, active work-item views, bottleneck pointer
- `StationSnapshot`: queue length, worker state, processed count, utilization, queue wait, and processing times
- `WorkItemSnapshot`: current state, current stage or station, timestamps, progress, and renderable status
- `KpiSnapshot`: throughput, lead time, WIP, bottleneck, stage metrics, and compact trend points
- `AlertSnapshot`: current active warnings or later disruptions
- `SnapshotMetadata`: publication metadata such as publish time, reason, or schema version

## Copy Versus Reference Rules

Recommended default:

- snapshots must be logically immutable and self-contained for consumers
- mutable runtime collections are never exposed by reference
- small scalar values and compact records are copied into the snapshot
- run-scoped immutable scenario or layout data may be referenced only if guaranteed immutable for the full run

## Timeline Strategy

Recommended MVP strategy:

- keep one authoritative latest snapshot for live readers
- keep a run-scoped snapshot timeline for playback-oriented consumers such as the desktop UI
- retain all snapshots in memory for MVP if cadence remains coarse enough
- add compaction or bounded retention only when cadence or run duration requires it

## KPI Definitions

Recommended KPI set:

| KPI | Definition | Calculation approach |
|---|---|---|
| Throughput | Completed work items per simulated time unit | `completed / elapsed simulated time` |
| Average lead time | Mean duration from creation to completion | average across completed work items |
| WIP | Work items currently not completed | `created - completed` or active count |
| Queue length per stage | Work items currently waiting in the shared stage queue | current queue count |
| Average queue wait per stage | Mean wait before processing starts | derived from tracking history |
| Average processing time per stage | Mean actual processing duration | derived from completion history |
| Utilization per stage | Busy worker time divided by available worker time | cumulative busy time over available time |
| Bottleneck indicator | Most constrained stage at the snapshot time | weighted score from queue pressure and utilization |

Recommended formulas:

```text
ThroughputPerHour = OrdersCompleted / max(ElapsedSimulationHours, epsilon)

AverageLeadTime = SumCompletedLeadTimes / max(OrdersCompleted, 1)

CurrentWip = OrdersCreated - OrdersCompleted

StageUtilization = StageBusyTime / max(StageWorkerCount * ElapsedSimulationTime, epsilon)

AverageQueueWait(stage) = SumQueueWaits(stage) / max(StartedProcessCount(stage), 1)

AverageProcessingTime(stage) = SumProcessingDurations(stage) / max(CompletedProcessCount(stage), 1)
```

## Bottleneck Scoring

Recommended MVP heuristic:

```text
BottleneckScore(stage) =
    QueueLengthWeight * NormalizedQueueLength
  + UtilizationWeight * NormalizedUtilization
```

Suggested defaults:

- `QueueLengthWeight = 0.6`
- `UtilizationWeight = 0.4`

## KPI Calculation Timing

Recommended two-phase model:

1. event handlers and orchestrators update tracking facts and compact KPI aggregates continuously
2. snapshot publication projects those aggregates into `KpiSnapshot`

This keeps runtime cost predictable while preserving one consistent KPI truth for all consumers.

## KPI Collector State Direction

The KPI collector should own only compact aggregates and short trend buffers, not full runtime history.

Recommended internal shape:

```csharp
public sealed class KpiCollectorState
{
    public long OrdersCreated { get; private set; }
    public long OrdersCompleted { get; private set; }
    public TimeSpan SumCompletedLeadTimes { get; private set; }
    public TimeSpan? MinLeadTime { get; private set; }
    public TimeSpan? MaxLeadTime { get; private set; }
    public int CurrentWorkInProgress { get; private set; }
    public int PeakWorkInProgress { get; private set; }
    public IReadOnlyDictionary<string, StationTracking> Stations { get; }
    public IReadOnlyDictionary<string, StageTracking> Stages { get; }
    public IReadOnlyList<KpiTrendPointInternal> TrendBuffer { get; }
}
```

Recommended ownership rule:

- detailed per-work-item history remains in tracking structures
- station and stage-level facts remain in their dedicated tracking models
- the KPI collector keeps only the compact aggregates needed for fast projection into `KpiSnapshot`
- bottleneck scoring and trend generation should read these aggregates instead of replaying raw history at publish time
