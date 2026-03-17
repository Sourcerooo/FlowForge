namespace FlowForge.Simulation.Runtime.Entities;

public readonly record struct ProcessConfigurationId(Guid Value)
{
  public static ProcessConfigurationId NewId() => new ProcessConfigurationId(Guid.NewGuid());
}
