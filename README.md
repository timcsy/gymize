# marenv - Markov Environment

Reinforcement and Imitation Learning API with Gymnasium and PettingZoo.

## Field Mapping

The developer can use the field string to map the data to the gym-style space data.

The following is an valid example:

```
agent1@agent2@.key.0[12]["camera"]['front'][right](2)[1:10:2=24:29 & 10 & 11=0 & 12=3]
```

### Field String Rules
- Path Position
  - Using `[index]` to represent Sequence
    - `index` is an integer
  - Using `(index)` to represent Tuple
    - `index` is an integer
  - Using `{key}`, `["key"]`, `['key']`, `.key` to represent Dict
  - Using `[slice1, slice2, ...]` or `(slice1, slice2, ...)` to represent Fundamental Spaces like Box, Discrete, MultiBinary, MultiDiscrete
    - `slice` can be an `index` or Python-like slice `start:end:step`
  - Using `.` to represent upper position of the path position
- Mapping
  - Using assignment `=` to map the position between the real data and the field structure
  - One can omit assignment `=` to retrieve from the data beginning with first position
- Agent name
  - Using `@` in the beginning to represent the root (directly under the agent)
  - Using `agent@` to assign the agent name in the beginning
  - Using `@@` in the beginning means for all agents
  - Using `@@agent1@agent2@` in the beginning means for all agents, except `agent1`, `agent2`