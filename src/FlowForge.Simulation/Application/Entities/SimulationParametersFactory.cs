using FlowForge.Domain.Process.Entities;
using FlowForge.Domain.Process.ValueObjects;
using FlowForge.Domain.Scenarios.Entities;

namespace FlowForge.Simulation.Application.Entities;

internal class SimulationParametersFactory
{
  internal static ProcessConfiguration Create()
  {
    var stage1Id = StageId.NewId();
    var stage2Id = StageId.NewId();
    return ProcessConfiguration.Create(
          "test-process", "Test Process", DateTime.UtcNow, TimeSpan.FromHours(2),
          new ArrivalProfileDefinition(TimeSpan.FromMinutes(60), 10, 10),
          new List<StageDefinition>
          {
            new StageDefinition(stage1Id, "stage-1", "Stage 1", 1, new List<StationDefinition>{
              new StationDefinition(StationId.NewId(), stage1Id, "station-1-1", "Station 1-1", 3, TimeSpan.FromMinutes(5))
            }),
            new StageDefinition(stage2Id, "stage-2", "Stage 2", 2, new List<StationDefinition>{
              new StationDefinition(StationId.NewId(), stage2Id, "station-2-1", "Station 2-1", 3, TimeSpan.FromMinutes(5))
            }),
          });
  }
}
