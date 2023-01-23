# marenv - Markov Environment

Reinforcement and Imitation Learning API with Gymnasium and PettingZoo.

## Location Mapping

The developer can use the "location string" to map the data to the gym-style space data.

The following is an valid example:

```
agent1@agent2@.key.0[12]["camera"]['front'][right](2)[1:10:2=24:29 & 10 & 11=0 & 12=3]
```

### Location String Rules
- Path Syntax
  - Using `[index]` to represent Sequence
    - `index` is an integer
  - Using `(index)` to represent Tuple
    - `index` is an integer
  - Using `{key}` or `["key"]` or `['key']` or `.key` to represent Dict
    - `key` in `{key}` and `.key` should be a string begin with letters or `_`, can follow letters or number or `_`
  - Using `[slice1, slice2, ...]` or `(slice1, slice2, ...)` to represent Fundamental Spaces like Box, Discrete, MultiBinary, MultiDiscrete
    - `slice` can be an `index` or Python-like slice `start:end:step`
    - `slice` can only be at the end of the path
  - Using `.` to get upper level of the path (just like relative file path)
- Mapping
  - Using assignment `=` to map the key or index between the real data and the field structure
  - One can omit assignment `=` to retrieve from the data beginning with index 0 or the first key if is iterable, otherwise from the data directly
  - `=` can only be at the end of the path
- Agent name
  - Using `@` in the beginning to represent the root (directly under the agent)
  - Using `agent@` to assign the agent name in the beginning
  - Using `@@` in the beginning means for all agents
  - Using `@@agent1@agent2@` in the beginning means for all agents, except `agent1`, `agent2`

### Attribute
Define the Observer in the class or struct, using the attributes.

### Sensor Component