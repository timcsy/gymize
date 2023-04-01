# Gymize - Unity Environment with Gymnasium and PettingZoo

Reinforcement and Imitation Learning API with Gymnasium and PettingZoo.

## Location Mapping

The developer can use the "location string" to map the data to the gym-style space data.

The followings are valid examples:

```
agent1@agent2@.key.0[12]["camera"]['front'][right](2)[87]
agent1@agent2@.key.0[12]["camera"]['front'][right](2)[1:10:2=24:29 & 11=0 & 12=3]
```

### Location String Rules
- Path Syntax
  - In the following, `NAME` represents a string begin with letters or `_`, can follow letters or numbers or `_`
  - Using `{key}` or `["key"]` or `['key']` or `.key` to represent Dict
    - `key` in `{key}` should be a `NAME`
    - `key` in `.key` should be composite with letters or numbers or `_`
  - Using `(index)` to represent Tuple
    - If `index` is an integer, it represent Tuple
    - If `index` is a `NAME`, it will convert to a Dict
  - Using `[index]` to represent Sequence
    - If `index` is an integer, it represent Sequence
    - If `index` is a `NAME`, it will convert to a Dict
  - Using `[slice1, slice2, ...]` or `(slice1, slice2, ...)` to represent Fundamental Spaces like Box, Discrete, MultiBinary, MultiDiscrete
    - `slice` can be an `index` or Python-like slice `start:end:step`
    - `slice` can only be at the end of the path
  - Using `.` to get upper level of the path (just like relative file path)
- Mapping
  - Using assignment `=` to map the key or index between the real data and the source data by the structure
  - `=` can only be at the end of the path
  - If there is no mapping at the end of the location string, then the source data will be viewed as indivisible and stored in the given destination
- Agent name
  - Using `@` in the beginning to represent the root (directly under the agent)
  - Using `agent@` to assign the agent name in the beginning
  - Using `@@` in the beginning means for all agents
  - Using `@@agent1@agent2@` in the beginning means for all agents, except `agent1`, `agent2`

### Attribute
Define the Observer in the class or struct, using the attributes.

### Sensor Component