namespace FlowForge.Simulation.Runtime.Enums;

public interface ISimulationState
{
  public void AdvanceTo(TimeSpan toTime);
  public long GetNextSequenceNumber();
}

public record class SimulationState : ISimulationState
{
  private TimeSpan _currentTimeElapsed;
  private long _sequenceNumber = 0;
  public void AdvanceTo(TimeSpan toTime)
  {
    _currentTimeElapsed = toTime;
  }

  public long GetNextSequenceNumber()
  {
    return _sequenceNumber++;
  }
}
