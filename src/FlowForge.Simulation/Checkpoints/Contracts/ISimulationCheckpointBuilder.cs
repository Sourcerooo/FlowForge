using FlowForge.Simulation.Checkpoints.Documents;

namespace FlowForge.Simulation.Checkpoints.Contracts;

public interface ISimulationCheckpointBuilder
{
    public SimulationCheckpointDocument Build(SimulationStateDocument state);
}
