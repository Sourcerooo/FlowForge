namespace FlowForge.Simulation.Runtime.Enums;

public interface ISimulationState
{
  public void AdvanceTo(TimeSpan toTime);
  public long GetNextSequenceNumber();
}

public record class SimulationState : ISimulationState
{
  private long _sequenceNumber = 0;
  public TimeSpan CurrentTime { get; private set; } = TimeSpan.FromSeconds(0);

  public void AdvanceTo(TimeSpan toTime)
  {
    CurrentTime = toTime;
  }

  public long GetNextSequenceNumber()
  {
    return _sequenceNumber++;
  }


}
