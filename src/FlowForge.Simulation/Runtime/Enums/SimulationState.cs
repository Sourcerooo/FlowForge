namespace FlowForge.Simulation.Runtime.Enums;

public interface ISimulationState
{
  public void AdvanceTo(TimeSpan toTime);
}

public record class SimulationState : ISimulationState
{
  private TimeSpan _currentTimeElapsed;
  public void AdvanceTo(TimeSpan toTime)
  {
    _currentTimeElapsed = toTime;
  }
}
