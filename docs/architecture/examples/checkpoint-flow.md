# Checkpoint Flow Example

This document is informative only.

## Save Flow

```text
User action / CLI command / API request
  -> Application use case
  -> simulation maps live runtime to SimulationExecutionState
  -> ISimulationCheckpointStore.SaveAsync(state, filePath)
  -> ISimulationCheckpointBuilder.Build(SimulationExecutionState)
  -> SimulationCheckpointDocument
  -> Infrastructure JSON adapter
  -> one *.flowforge-run.json file
```

## Load Flow

```text
User action / CLI command / API request
  -> Application use case
  -> ISimulationCheckpointStore.LoadAsync(filePath)
  -> SimulationCheckpointDocument
  -> ISimulationStateBuilder.Build(checkpoint)
  -> SimulationExecutionState
  -> simulation maps SimulationExecutionState to live execution context
  -> ISimulationRunner.RunAsync(executionContext)
```

## Boundary Reminder

- application decides when save or load occurs
- infrastructure owns the physical file access
- simulation owns the mapping between portable documents and live runtime objects
