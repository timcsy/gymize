'''
$ python test_obs.py 
Connected to Signal Server: ws://localhost:50864/
Please start the Unity game in the Unity Editor or open the game manually!
/.../gymnasium/utils/passive_env_checker.py:35: UserWarning: WARN: A Box observation space has an unconventional shape (neither an image, nor a 1D vector). We recommend flattening the observation to have only a 1D vector or use a custom policy to properly process the data. Actual observation shape: (2, 2)
  logger.warn(
i = 0
Start Peer Server: ws://localhost:54337
OrderedDict([('CameraFront', array([[[ 98, 123, 163],
        [ 98, 123, 163],
        [ 98, 123, 163],
        ...,
        [ 95, 120, 160],
        [ 95, 120, 160],
        [ 95, 120, 160]],

       [[ 98, 123, 163],
        [ 98, 123, 163],
        [ 98, 123, 163],
        ...,
        [ 95, 120, 160],
        [ 95, 120, 160],
        [ 95, 120, 160]],

       [[ 98, 123, 163],
        [ 98, 123, 163],
        [ 98, 123, 163],
        ...,
        [ 95, 120, 160],
        [ 95, 120, 160],
        [ 95, 120, 160]],

       ...,

       [[106,  97,  92],
        [106,  97,  92],
        [106,  97,  92],
        ...,
        [106,  97,  92],
        [106,  97,  92],
        [106,  97,  92]],

       [[106,  97,  92],
        [106,  97,  92],
        [106,  97,  92],
        ...,
        [106,  97,  92],
        [106,  97,  92],
        [106,  97,  92]],

       [[106,  97,  92],
        [106,  97,  92],
        [106,  97,  92],
        ...,
        [106,  97,  92],
        [106,  97,  92],
        [106,  97,  92]]], dtype=uint8)), ('ddd', OrderedDict([('stringField', array([104, 101, 108, 108, 111], dtype=uint16))])), ('floatVarProp', array([3.], dtype=float32)), ('int2ArrayAttr', array([[1, 2],
       [4, 5]], dtype=int32)), ('intArrayArrayAttr', array([[2, 3],
       [5, 6]], dtype=int32)), ('intArrayAttr', array([4, 5, 6], dtype=int32)), ('intListAttr', array([1, 2, 3], dtype=int32)), ('intVarField', array([23], dtype=int32)), ('textField', 'hello'), ('varBool', array([ True])), ('varByte', array([255], dtype=uint8)), ('varDecimal', array([3.14159265])), ('varEnum', array([1], dtype=int32)), ('varFloat32', array([-inf], dtype=float32)), ('varFloat64', array([-inf])), ('varInt16', array([-32768], dtype=int16)), ('varInt32', array([-2147483648], dtype=int32)), ('varInt64', array([-9223372036854775808])), ('varQuaternion', array([1., 0., 0., 0.], dtype=float32)), ('varUInt16', array([65535], dtype=uint16)), ('varUInt32', array([4294967295], dtype=uint32)), ('varUInt64', array([18446744073709551615], dtype=uint64)), ('varVector2', array([2.1, 2.2], dtype=float32)), ('varVector2Int', array([21, 22], dtype=int32)), ('varVector3', array([0., 0., 0.], dtype=float32)), ('varVector3Int', array([31, 32, 33], dtype=int32)), ('varVector4', array([4.4, 4.1, 4.2, 4.3], dtype=float32))])
i = 1
OrderedDict([('CameraFront', array([[[ 98, 123, 163],
        [ 98, 123, 163],
        [ 98, 123, 163],
        ...,
        [ 95, 120, 160],
        [ 95, 120, 160],
        [ 95, 120, 160]],

       [[ 98, 123, 163],
        [ 98, 123, 163],
        [ 98, 123, 163],
        ...,
        [ 95, 120, 160],
        [ 95, 120, 160],
        [ 95, 120, 160]],

       [[ 98, 123, 163],
        [ 98, 123, 163],
        [ 98, 123, 163],
        ...,
        [ 95, 120, 160],
        [ 95, 120, 160],
        [ 95, 120, 160]],

       ...,

       [[106,  97,  92],
        [106,  97,  92],
        [106,  97,  92],
        ...,
        [106,  97,  92],
        [106,  97,  92],
        [106,  97,  92]],

       [[106,  97,  92],
        [106,  97,  92],
        [106,  97,  92],
        ...,
        [106,  97,  92],
        [106,  97,  92],
        [106,  97,  92]],

       [[106,  97,  92],
        [106,  97,  92],
        [106,  97,  92],
        ...,
        [106,  97,  92],
        [106,  97,  92],
        [106,  97,  92]]], dtype=uint8)), ('ddd', OrderedDict([('stringField', array([104, 101, 108, 108, 111], dtype=uint16))])), ('floatVarProp', array([3.], dtype=float32)), ('int2ArrayAttr', array([[1, 2],
       [4, 5]], dtype=int32)), ('intArrayArrayAttr', array([[2, 3],
       [5, 6]], dtype=int32)), ('intArrayAttr', array([4, 5, 6], dtype=int32)), ('intListAttr', array([1, 2, 3], dtype=int32)), ('intVarField', array([23], dtype=int32)), ('textField', 'hello'), ('varBool', array([ True])), ('varByte', array([255], dtype=uint8)), ('varDecimal', array([3.14159265])), ('varEnum', array([1], dtype=int32)), ('varFloat32', array([-inf], dtype=float32)), ('varFloat64', array([-inf])), ('varInt16', array([-32768], dtype=int16)), ('varInt32', array([-2147483648], dtype=int32)), ('varInt64', array([-9223372036854775808])), ('varQuaternion', array([1., 0., 0., 0.], dtype=float32)), ('varUInt16', array([65535], dtype=uint16)), ('varUInt32', array([4294967295], dtype=uint32)), ('varUInt64', array([18446744073709551615], dtype=uint64)), ('varVector2', array([2.1, 2.2], dtype=float32)), ('varVector2Int', array([21, 22], dtype=int32)), ('varVector3', array([0., 0., 0.], dtype=float32)), ('varVector3Int', array([31, 32, 33], dtype=int32)), ('varVector4', array([4.4, 4.1, 4.2, 4.3], dtype=float32))])
'''

import os
import sys

import gymnasium as gym
from gymnasium import spaces

import numpy as np

if __name__ == '__main__':
    file_name = None
    if len(sys.argv) > 1:
        file_name = sys.argv[1]
    
    if not os.path.exists('imgs'):
        os.makedirs('imgs')
    
    observation_space = spaces.Dict(
        {
            'CameraFront': spaces.Box(0, 255, shape=(540, 960, 3,), dtype=np.uint8),
            'intVarField': spaces.Box(-2147483648, 2147483647, dtype=np.int32),
            'floatVarProp': spaces.Box(-np.inf, np.inf, dtype=np.float32),
            'intArrayAttr': spaces.Box(-2147483648, 2147483647, shape=(3,), dtype=np.int32),
            'intListAttr': spaces.Box(-2147483648, 2147483647, shape=(3,), dtype=np.int32),
            'int2ArrayAttr': spaces.Box(-2147483648, 2147483647, shape=(2, 2,), dtype=np.int32),
            'intArrayArrayAttr': spaces.Box(-2147483648, 2147483647, shape=(2, 2,), dtype=np.int32),
            'ddd': spaces.Dict(
                {
                    'stringField': spaces.Box(0, 65535, shape=(5,), dtype=np.uint16),
                }
            ),
            'textField': spaces.Text(100),
            'varEnum': spaces.Box(-2147483648, 2147483647, dtype=np.int32),
            'varBool': spaces.MultiBinary(1),
            'varInt16': spaces.Box(-32768, 32767, dtype=np.int16),
            'varInt32': spaces.Box(-2147483648, 2147483647, dtype=np.int32),
            'varInt64': spaces.Box(-9223372036854775808, 9223372036854775807, dtype=np.int64),
            'varByte': spaces.Box(0, 255, dtype=np.uint8),
            'varUInt16': spaces.Box(0, 65535, dtype=np.uint16),
            'varUInt32': spaces.Box(0, 4294967295, dtype=np.uint32),
            'varUInt64': spaces.Box(0, 18446744073709551615, dtype=np.uint64),
            'varFloat32': spaces.Box(-np.inf, np.inf, dtype=np.float32),
            'varFloat64': spaces.Box(-np.inf, np.inf, dtype=np.float64),
            'varDecimal': spaces.Box(-np.inf, np.inf, dtype=np.float64),
            'varVector2': spaces.Box(-np.inf, np.inf, shape=(2,), dtype=np.float32),
            'varVector2Int': spaces.Box(-2147483648, 2147483647, shape=(2,), dtype=np.int32),
            'varVector3': spaces.Box(-np.inf, np.inf, shape=(3,), dtype=np.float32),
            'varVector3Int': spaces.Box(-2147483648, 2147483647, shape=(3,), dtype=np.int32),
            'varVector4': spaces.Box(-np.inf, np.inf, shape=(4,), dtype=np.float32),
            'varQuaternion': spaces.Box(-np.inf, np.inf, shape=(4,), dtype=np.float32),
        }
    )
    action_space = spaces.Discrete(4)

    env = gym.make('gymize/Unity-v0', name='kart', file_name=file_name, observation_space=observation_space, action_space=action_space)
    observation, info = env.reset()

    for i in range(2):
        action = env.action_space.sample()  # agent policy that uses the observation and info
        print(f'i = {i}')
        observation, reward, terminated, truncated, info = env.step(action)
        print(observation)

        if terminated or truncated:
            observation, info = env.reset()

    env.close()