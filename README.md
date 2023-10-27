# Gymize - Gym Reinforcement Learning with Unity 3D

Unity and Python Reinforcement and Imitation Learning with Gymnasium and PettingZoo API.


## Installations

### Installation for Python 3

```
pip install gymize
```

If you want to render videos, please install [ffmpeg](https://ffmpeg.org/download.html) additionally.

You can also install ffmpeg with Anaconda:
```
conda install -c conda-forge ffmpeg
```

### Installation for Unity

Go to Window -> Package Manager -> Add package from git URL...

Then paste the following git URL:
```
https://github.com/timcsy/gymize.git?path=/unity
```

If git hasn't been installed, then you can download gymize first and install it from disk, specify the path to `/unity/package.json`, or you can install from tarball, downloaded from Releases.


## Usages

### Usage for Python 3

1. Assign the `env_name`, which should be same as in the Unity. For example, `kart`.
2. `file_name` is the path to the built Unity game, or leave `None` if using Unity Editor.
3. Define the observation space in the format of [Gym Spaces](https://gymnasium.farama.org/api/spaces/).
4. Define the action space in the format of [Gym Spaces](https://gymnasium.farama.org/api/spaces/).
5. Assign `render_mode='video'` if you want to record video from Unity, otherwise omit `render_mode`.
6. Assign `views=['', ...]` if you want to record video from Unity, which given a list of view names, the empty string will be the default view, otherwise omit `views`.
7. Make the Gymnasium or PettingZoo environment by the following commands:

Single-Agent with Gymnasium API:
```
import gymnasium as gym
import gymize

env = gym.make(
    'gymize/Unity-v0',
    env_name='<your env name>',
    file_name=file_name,
    observation_space=observation_space,
    action_space=action_space,
    render_mode='<render_mode>',
    views=['', ...]
)
```

Multi-Agents with PettingZoo AEC API:
```
from gymize.envs import UnityAECEnv

env = UnityAECEnv(
    env_name='<your env name>',
    file_name=file_name,
    observation_spaces=observation_spaces,
    action_spaces=action_spaces,
    render_mode='<render_mode>',
    views=['', ...]
)
```

Multi-Agents with PettingZoo Parallel API:
```
from gymize.envs import UnityParallelEnv

env = UnityParallelEnv(
    env_name='<your env name>',
    file_name=file_name,
    observation_spaces=observation_spaces,
    action_spaces=action_spaces,
    render_mode='<render_mode>',
    views=['', ...]
)
```

Well done! Now you can use the environment as the gym environment!

The environment `env` will have some additional methods other than Gymnasium or PettingZoo:
- `env.unwrapped.send_info(info, agent=None)`
    - At anytime, you can send information through `info` parameter in the form of Gymize Instance (see below) to Unity side.
    - The `agent` parameter is the agent name that will receive info (by `Gymize.Agent.OnInfo()` method in the Unity side), or `None` for the environment to receive info (by `Gymize.GymEnv.OnInfo += (info) => {}` listener in the Unity side).
- `env.unwrapped.begin_render(screen_width=-1, screen_height=-1, fullscreen=False)`
    - Begin to record video in the Unity.
    - `screen_width` and `screen_height` is to set the width and height of the window, leave `-1` if you want the default window size.
    - `fullscreen` is to set whether Unity is fullscreen.
- `env.unwrapped.end_render()`
    - Stop to record video in the Unity.
- `env.unwrapped.render_all(video_paths={})`
    - Render the videos which name and path are given by key value pairs in `video_path`.
    - Using `None` as path for binary video object.
- `env.unwrapped.render()`
    - Only render the default video, and return a binary video object.

If you want to open the signaling service only, you can run `gymize-signaling` at the command line.

### Usage for Unity

1. Create Gym Manager Component
    - Add a game object in the scene.
    - Add a `Gym Manager` Component to the game object.
    - Fill in the `Env Name` property with the name of the environment, which should be same as in the Python. For example, `kart`.
2. Implement a class that inherit the `Agent` class: `class MyAgent : Agent {}`
    - Note that the `Agent` class is inheritted from `MonoBehaviour` class.
        - Override these listeners
        - `public override void OnReset() {}`
        - `public override void OnAction(object obj) {}`
        - `public override void OnInfo(object obj) {}`
      - Add `Terminate()`, `Truncate()` at the proper place.
      - For example:
        ```
        public class MyAgent : Agent
        {
            [Obs]
            private float pi = 3.14159f;

            private int m_Count;
            private float m_Speed;
            private string m_NickName;

            public override void OnReset()
            {
                // Reload the Unity scene when getting the reset signal from Python
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }

            public override void OnAction(object action)
            {
                Dictionary<string, object> actions = action as Dictionary<string, object>;
                List<object> arr = actions["arr"] as List<object>;

                Debug.Log((long)arr[0]); // arr[0] is Discrete value
                m_Count = Convert.ToInt32(arr[0]); // arr[0] is Discrete value
                m_Speed = Convert.ToSingle(actions["speed"]); // actions["speed"] is float value
            }

            public override void OnInfo(object info)
            {
                m_NickName = (string)info;
            }

            void Update()
            {
                // Terminate the game if collision occurred
                if (m_Collision)
                {
                    // This method will tell the Python side to terminate the env
                    Terminate();
                }
            }
        }
        ```
    - Add this Agent Component to the game object, and fill in the `Name` property with the name of the agent, using `agent` for single agent.
3. Add Observers
    - Refer [below](#Locator) to learn more about the concept of "Locator".
    - Using Attributes in the Agent class, assign the Locator or use the default Locator, for example:
        ```
        [Obs] // Using the default Locator ".UsedTime", which is same as the field name.
        private float UsedTime;

        [Obs(".Progress")]
        private float m_Progress;

        [Box]
        private float Distance;

        [Box(".Height=$")]
        private float m_Height;
        ```
    - You can use `Obs` to use the default Gym space type depending on the source variable, or you can use `Box`(for Tensor, including MultiBinrary and MultiDiscrete), `Discrete`, `Text`, `Dict`, `List`, `Graph` attributes, see more details in the [Reflection](unity/Runtime/Agent/Observation/Reflection/) folder, check out [TestAgentInstance.cs](unity/Tests/Runtime/TestAgentInstance.cs) for more examples.
    - Add Sensor Component in the scene.
        - Inheritted from `SensorComponent` class. `CameraSensor` is an example of a sensor component.
        - Make sure that the `public override IInstance GetObservation() {}` method is implemented in the Sensor Component.
        - Assign proper Locator in the Unity Editor.
4. Add Gym Render Recorder Component to the scene if needed
    - The `Name` property can be empty or the name of the view.
    - At the Python side, set `render_mode='video'` if you want to render videos.
5. You can disable the `Gym Manager` component in the Unity Editor to develop the game without Python connection and play the game manually, it is useful for debugging.

!!! Remember to close the channel in MonoBehaviour.OnApplicationQuit !!!


## Gymize Instance

The instance generated from the action, observation space or info is called "Gymize Instance".

Gymize Instance is defined in the [space.proto](proto/definitions/space.proto), which describes how the Gymize exchange data between Unity and Python, using [Protocol Buffers 3](https://protobuf.dev/programming-guides/proto3/). Most of which originates from the [Gym Spaces](https://gymnasium.farama.org/api/spaces/).

In Unity, check out [GymInstance.cs](unity/Runtime/Space/GymInstance.cs) for more information about how to convert the object into a meaningful type. In Python, you can treat the instance as usual object.

### Fundamental Instance

- `Tensor`: numpy array, with dtype and shape
    - Corresponding to the Gym Spaces as `Box`, `MultiBinrary`, `MultiDiscrete`
- `Discrete`: int64
    - Corresponding to the Gym Spaces as `Discrete`
- `Text`: string
    - Corresponding to the Gym Spaces as `Text`

### Composite Instance

- `Dict`: `key`, `value` pairs mapping. Type of `key` is string, `value` is Gymize Instance
    - Corresponding to the Gym Spaces as `Dict`
- `List`: array of Gymize Instance
    - Corresponding to the Gym Spaces as `Tuple`, `Sequence`
- `Graph`: including three tensor objects, which are `nodes`, `edges` and `edge_links`
    - `nodes`: the numeric information of nodes
    - `edges`: the numeric information of edges
    - `edge_links`: the list of edges represent by node pairs (the node index begins with 0)
    - Corresponding to the Gym Spaces as `Graph`

### Additional Instance (Not defined in Gym Spaces)

- `Raw`: binary data
- `Image`: image data, include format (PNG, JPG, ...), binary data, dtype, shape, axis permutation
- `Float`: double precision floating number
- `Boolean`: boolean value (true/false)
- `JSON`: JSON after stringifying


## Locator

"Locator" maps the observations collected from the Unity side to the specified location of Python side observation data, which is transferred by Gymize Instance.

The followings are valid examples:

example 1:
```
.UsedTime = $
```

example 2:
```
.Progress
```

example 3:
```
@.Rays
```

example 4:
```
agent1@agent2@.key.0[12]["camera"]['front'][right][87](2)
```

example 5:
```
@@agent3@agent4@["camera"](1:10:2) = $(24:29) & @[11]=$[0] & @.key = $(3:8)
```

For more examples, check out [TestLocator.cs](unity/Tests/Runtime/TestLocator.cs) and [TestAgentInstance.cs](unity/Tests/Runtime/TestAgentInstance.cs).

### Structure of Locator

- A `Locator` is a sequence of `Mapping`s.
- A `Mapping` consists of `Agent`, `Destination`, and `Source`.
    - `Agent` has four kinds: "all agents", "list agents", "root agent", and "omitted".
    - `Destination` and `Source` are in the form of `Selector`.
- A `Selector` can act on the following types: `Dict`, `Tuple`, `Sequence`, `Tensor`.
    - "key" selector is for `Dict`.
    - "index" or "slices" selector is for `Tuple`, `Sequence`, `Tensor`.
- A `Slice` is Python-like or Numpy-like, which has "start", "stop", and "step".
    - Usually we write `start:stop:step`, e.g. `1:10:2`.
    - There are some special cases: "index", "ellipsis", and "new axis".

### Syntax for Locator

- Syntax for `Locator`
    - Using `&` to connect different `Mapping`s.
    - e.g. `.field1 & .field2=$ & @.field3=$["key"]`
- Syntax of `Mapping`
    - {`Agent`} `Selectors(Destination)` {`=$` `Selectors(Source)`}
        - { } means can be omitted.
        - If you omit `Agent`, then the Locator will become relative w.r.t Agent.
        - If you omit `=$` `Selectors(Source)`, it is same as `=$`.
        - `=` means mapping or assignment. It will map the source (right hand side) to the destination (left hand side).
        - `$` means the Unity side observation (variable or sensor data).
- Syntax for `Agent`
    - Using `agent@` to assign the agent name.
    - Using `agent1@agent2@` to assign the agent names.
    - Using `@@` means for all agents.
    - Using `@@agent3@agent4@` means for all agents, except `agent3` and `agent4`.
    - Using `@` to represent the root agent itself.
    - You can omit the `Agent` to use relative location.
- Syntax for `Selector`
    - For `Dict`: `.key`, `['key']`, `["key"]`, or `[key]`
    - For `Tuple`: `[index]` or `[slice]`
    - For `Sequence`: `[]`, means append
    - For `Tensor`: `(Slice)` or `(Slice1, Slice2, ..., SliceN)`
        - The several `Tensor` selectors have to put at the end of the selector sequence.
- Syntax for `Slice`
    - Same syntax as Python or numpy.
    - `start:stop:step`, e.g. `1:10:2`.
        - omitted "step", it becomes `1:10`.
        - omitted "stop", it becomes `1:`.
        - omitted "start", it becomes `:`.
    - You can just use an integer to represent the index (negative integer means counting from the end).
    - You can use `...` to represent ellipsis.
    - You can use `newaxis` or `np.newaxis` to represent new axis.

See [locator.bnf](locator.bnf) for more information about the syntax.


## Known issues

- Gymize get the observation space type information by generating a sample instance, so it may not get the type information of `Sequence` if it samples a empty array.
- On the Python side, it may cause some problems if you edit the observation data directly, because it is not a copy, it is a reference object.
- Dict source with different keys but with same destination Locator may not merged correctly, check out `space.py` for more details.
- If you run the built Unity Application and it fails after first OnReset, try:
    - It may be this issue: `ArgumentNullException: Value cannot be null. Parameter name: shader`.
    - Go to: Edit -> ProjectSettings -> Graphics
    - Change the size of Always Included Shaders, and add the `Runtime/Space/Grayscale.shader` into it.