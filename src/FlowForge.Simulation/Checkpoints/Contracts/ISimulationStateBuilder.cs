using FlowForge.Simulation.Checkpoints.Documents;

namespace FlowForge.Simulation.Checkpoints.Contracts;

public interface ISimulationStateBuilder
{
    public SimulationStateDocument Build(SimulationCheckpointDocument checkpoint);
}
