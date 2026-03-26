using System.Collections.Immutable;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.Scenarios.Entities;

namespace FlowForge.Domain.Process.Entities;

public sealed class ProcessConfiguration
{
  public ProcessConfigurationId ProcessConfigurationId { get; }
  public string ProcessKey { get; }
  public string Name { get; }
  public DateTime StartTime { get; }
  public TimeSpan PlannedDuration { get; }
  public ArrivalProfileDefinition ArrivalProfileDefinition { get; }
  public ImmutableArray<StageDefinition> Stages { get; }

  private ProcessConfiguration(
      string processKey,
      string name,
      DateTime startTime,
      TimeSpan plannedDuration,
      ArrivalProfileDefinition arrivalProfileDefinition,
      ImmutableArray<StageDefinition> stages)
  {
    ProcessConfigurationId = ProcessConfigurationId.NewId();
    ProcessKey = processKey;
    Name = name;
    StartTime = startTime;
    PlannedDuration = plannedDuration;
    ArrivalProfileDefinition = arrivalProfileDefinition;
    Stages = stages;
  }

  public static ProcessConfiguration Create(
      string processKey,
      string name,
      DateTime startTime,
      TimeSpan plannedDuration,
      ArrivalProfileDefinition arrivalProfileDefinition,
      IReadOnlyList<StageDefinition> stages)
  {
    var immutableStages = stages.OrderBy(stage => stage.Sequence).ToImmutableArray();
    return VerifyStages(immutableStages)
      ? new ProcessConfiguration(
                processKey,
                name,
                startTime,
                plannedDuration,
                arrivalProfileDefinition,
                immutableStages)
      : throw new ArgumentException("Stages and/or stations violate invariant constraints");
  }

  private static bool VerifyStages(IEnumerable<StageDefinition> stages)
  {
    return IsStageSequenceUnique(stages)
      && IsStationInStage(stages)
      && IsStationUnique(stages);
  }

  private static bool IsStageSequenceUnique(IEnumerable<StageDefinition> stages)
  {
    var isUnique = true;
    var sequenceSeen = new HashSet<int>();
    foreach (var stage in stages)
    {
      if (!sequenceSeen.Add(stage.Sequence))
      {
        isUnique = false;
        break;
      }
    }
    return isUnique;
  }

  private static bool IsStationUnique(IEnumerable<StageDefinition> stages)
  {
    bool isUnique = true;
    var stationIds = new HashSet<StationId>();
    foreach (var stage in stages)
    {
      foreach (var station in stage.Stations)
      {
        if (!stationIds.Add(station.StationId) || station.StageId != stage.StageId)
        {
          isUnique = false;
          break;
        }
      }
    }
    return isUnique;
  }

  private static bool IsStationInStage(IEnumerable<StageDefinition> stages)
  {
    foreach (var stage in stages)
    {
      if (stage.Stations.Count == 0)
      {
        return false;
      }
    }
    return true;
  }
}
