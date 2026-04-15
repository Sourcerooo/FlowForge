using FlowForge.Domain.Process.Entities;
using FlowForge.Simulation.Runtime.Contracts;
using FlowForge.Simulation.Runtime.Enums;
using FlowForge.Simulation.Runtime.ValueObjects;

namespace FlowForge.Simulation.Runtime.Entities;

public class SimulationContextBuilder() : ISimulationContextBuilder
{
  public SimulationExecutionContext Build(ProcessConfiguration processConfiguration)
  {
    return new SimulationExecutionContext(
      SimulationRunId.NewId(),
      new SimulationMetadata(DateTime.UtcNow, "test-scenario", "1.0.0", new SimulationRunOptions()),
      new SimulationState(),
      processConfiguration);

  }
}
