using FlowForge.Domain.Orders.ValueObjects;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Tests;

public sealed class RuntimeValueObjectTests
{
  [Fact]
  public void SimulationState_GetNextSequenceNumber_ReturnsIncreasingValues()
  {
    var state = new SimulationState();

    var first = state.GetNextSequenceNumber();
    var second = state.GetNextSequenceNumber();
    var third = state.GetNextSequenceNumber();

    Assert.Equal(0, first);
    Assert.Equal(1, second);
    Assert.Equal(2, third);
  }

  [Fact]
  public void SimulationState_AdvanceTo_UpdatesCurrentTime()
  {
    var state = new SimulationState();
    var target = TimeSpan.FromMinutes(42);

    state.AdvanceTo(target);

    Assert.Equal(target, state.CurrentTime);
  }

  [Fact]
  public void SimulationRunId_NewId_CreatesDistinctValues()
  {
    var first = SimulationRunId.NewId();
    var second = SimulationRunId.NewId();

    Assert.NotEqual(first, second);
    Assert.NotEqual(Guid.Empty.ToString(), first.ToString());
  }

  [Fact]
  public void StageQueueEntry_UsesValueEquality()
  {
    var trackingSubjectId = TrackingSubjectId.NewId();
    var first = new StageQueueEntry(trackingSubjectId, TimeSpan.FromMinutes(1));
    var second = new StageQueueEntry(trackingSubjectId, TimeSpan.FromMinutes(1));

    Assert.Equal(first, second);
  }

  [Fact]
  public void StationProcessingInfo_UsesValueEquality()
  {
    var trackingSubjectId = TrackingSubjectId.NewId();
    var first = new StationProcessingInfo(trackingSubjectId, 1, TimeSpan.FromMinutes(5), new ProcessingToken(6));
    var second = new StationProcessingInfo(trackingSubjectId, 1, TimeSpan.FromMinutes(5), new ProcessingToken(6));

    Assert.Equal(first, second);
  }
}
