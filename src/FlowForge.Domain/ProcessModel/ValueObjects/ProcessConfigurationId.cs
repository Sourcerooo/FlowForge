namespace FlowForge.Domain.ProcessModel.ValueObjects;

public readonly record struct ProcessConfigurationId(Guid Value)
{
  public static ProcessConfigurationId NewId() => new ProcessConfigurationId(Guid.NewGuid());
}
