from collections.abc import Mapping, Iterable
import io
import json
import numpy as np
from PIL import Image
from typing import Any, Dict, List

from gymnasium.spaces import GraphInstance

from gymize.proto.space_pb2 import InstanceProto, InstanceTypeProto, TensorProto, GraphProto, ImageProto
from gymize.proto.locator_pb2 import LocatorProto, SelectorProto, SelectorTypeProto, SliceProto


def merge(observations: Dict[str, Any], locator: LocatorProto, resource_proto: InstanceProto, renewed_list: set=None) -> Any:
    resource = from_proto(resource_proto)

    for mapping in locator.mappings:
        if not mapping.is_root:
            print('locator should be root')
        elif mapping.is_all_agents:
            for agent in observations.keys():
                if not agent in mapping.agents:
                    observations[agent] = assign(observations[agent], mapping.destination, resource, mapping.source, renewed_list)
        else:
            for agent in mapping.agents:
                observations[agent] = assign(observations[agent], mapping.destination, resource, mapping.source, renewed_list)
    
    return observations

def assign(observation: Any, destination: List[SelectorProto], resource: Any, source: List[SelectorProto], renewed_list: set=None) -> Any:
    rvalue = select(resource, source)
    
    if len(destination) == 0:
        return to_type(rvalue, observation)
    
    lvalue = select(observation, destination[:-1]) # with the last selector in it
    selector = destination[-1] # last selector

    if selector.type == SelectorTypeProto.SELECTOR_TYPE_PROTO_DICT:
        lvalue[selector.key] = to_type(rvalue, lvalue[selector.key])
    elif selector.type == SelectorTypeProto.SELECTOR_TYPE_PROTO_TUPLE:
        slices = to_slices(selector.slices)
        lvalue[slices[0]] = to_type(rvalue, lvalue[slices[0]])
    elif selector.type == SelectorTypeProto.SELECTOR_TYPE_PROTO_SEQUENCE:
        is_numpy = isinstance(lvalue, np.ndarray)
        reference = lvalue[0] if len(lvalue) > 0 else None # for type referencing
        
        path = b''.join([selector.SerializeToString() for selector in destination]) # path for the Sequence
        if renewed_list is not None and path not in renewed_list:
            # erase the old ones and mark the current list as renewed
            if is_numpy:
                lvalue = np.ndarray([0] + list(lvalue.shape[1:]), dtype=lvalue.dtype)
            else:
                lvalue.clear()
            renewed_list.add(path)
        
        rvalue = rvalue if reference is None else to_type(rvalue, reference)
        if is_numpy:
            lvalue = np.vstack([lvalue, [rvalue]])
            if len(destination) > 1:
                # in order not to overwrite the lvalue
                pre_lvalue = select(observation, destination[:-2]) # with the last 2 selector in it
                assign(pre_lvalue, [destination[-2]], lvalue, []) # change the value inside
        else:
            lvalue.append(rvalue)
    elif selector.type == SelectorTypeProto.SELECTOR_TYPE_PROTO_TENSOR:
        slices = to_slices(selector.slices)
        lvalue[slices] = to_type(rvalue, lvalue[slices])
    
    if len(destination) == 1:
        return lvalue

    return observation

def select(resource: Any, selectors: List[SelectorProto]) -> Any:
    obj = resource
    for selector in selectors:
        if selector.type == SelectorTypeProto.SELECTOR_TYPE_PROTO_DICT:
            obj = obj[selector.key]
        elif selector.type == SelectorTypeProto.SELECTOR_TYPE_PROTO_TUPLE:
            obj = obj[to_slices(selector.slices)[0]]
        elif selector.type == SelectorTypeProto.SELECTOR_TYPE_PROTO_TENSOR:
            obj = obj[to_slices(selector.slices)]
    return obj

def to_type(obj: Any, reference: Any) -> Any:
    '''
    To ensure the object matches the space type
    '''
    if type(reference) == np.ndarray:
        if np.isscalar(obj):
            return np.array([obj], dtype=reference.dtype)
        if type(obj) == np.ndarray:
            return obj.astype(reference.dtype)
        else:
            return np.array(obj, dtype=reference.dtype)
    elif type(reference) == np.int64:
        return np.int64(obj)
    elif type(reference) == str:
        return str(obj)
    elif type(reference) == list:
        if len(obj) == len(reference):
            return [ to_type(obj[i], reference[i]) for i in range(len(obj)) ]
        elif len(reference) > 0:
            return [ to_type(value, reference[0]) for value in obj ]
        else:
            return obj
    elif type(reference) == dict:
        return { key: to_type(value, reference[key]) for key, value in dict(obj).items() }
    elif type(reference) == GraphInstance:
        return obj
    else:
        return obj
    
def list_to_tuple(obj: Any):
    if type(obj) == np.ndarray or type(obj) == str or type(obj) == GraphInstance:
        return obj
    elif isinstance(obj, Mapping):
        return { key: list_to_tuple(value) for key, value in obj.items() }
    elif isinstance(obj, Iterable):
        return tuple(obj)
    else:
        return obj
    
def tuple_to_list(obj: Any):
    if type(obj) == np.ndarray or type(obj) == str or type(obj) == GraphInstance:
        return obj
    elif isinstance(obj, Mapping):
        return { key: tuple_to_list(value) for key, value in obj.items() }
    elif isinstance(obj, Iterable):
        return list(obj)
    else:
        return obj

def to_slices(slices_proto: List[SliceProto]) -> List[slice]:
    slices = []
    for slice_proto in slices_proto:
        if slice_proto.is_index:
            slices.append(slice_proto.start)
        elif slice_proto.is_ellipsis:
            slices.append(Ellipsis)
        elif slice_proto.is_new_axis:
            slices.append(None)
        else:
            start = None
            stop = None
            if slice_proto.has_start:
                start = slice_proto.start
            if slice_proto.has_stop:
                stop = slice_proto.stop
            slices.append(slice(start, stop, slice_proto.step))
    return tuple(slices)

def from_proto(instance_proto: InstanceProto) -> Any:
    if instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_RAW:
        return bytes(instance_proto.raw_data)
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_TENSOR:
        return to_ndarray(instance_proto.tensor)
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_DISCRETE:
        return int(instance_proto.discrete)
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_TEXT:
        return str(instance_proto.text)
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_DICT:
        return { key: from_proto(value) for key, value in instance_proto.dict.items() }
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_LIST:
        return [ from_proto(value) for value in instance_proto.list ]
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_GRAPH:
        return to_graph(instance_proto.graph)
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_IMAGE:
        return image_to_ndarray(instance_proto.image)
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_FLOAT:
        return float(instance_proto.float)
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_BOOL:
        return bool(instance_proto.boolean)
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_NULL:
        return None
    elif instance_proto.type == InstanceTypeProto.INSTANCE_TYPE_PROTO_JSON:
        return json.loads(instance_proto.json)
    else:
        print('unsupported instance protobuf')
    
def to_ndarray(tensor_proto: TensorProto) -> np.ndarray:
    return np.frombuffer(tensor_proto.data, dtype=tensor_proto.dtype).reshape(tensor_proto.shape)
    
def to_graph(graph_proto: GraphProto) -> GraphInstance:
    nodes = to_ndarray(graph_proto.nodes)
    edges = to_ndarray(graph_proto.edges)
    edge_links = to_ndarray(graph_proto.edge_links)
    return GraphInstance(nodes=nodes, edges=edges, edge_links=edge_links)

def image_to_ndarray(image_proto: ImageProto) -> np.ndarray:
    img = Image.open(io.BytesIO(image_proto.data))
    if len(image_proto.shape) > 1:
        img = img.resize((image_proto.shape[1], image_proto.shape[0],))
    dtype = image_proto.dtype
    if dtype == '':
        dtype = np.uint8
    image_ndarray = np.array(img, dtype=dtype)
    if len(image_proto.transpose_axes) > 0:
        image_ndarray = image_ndarray.transpose(image_proto.transpose_axes)
    return image_ndarray

def save_image(box: np.ndarray, path: str):
    img = Image.fromarray(box)
    img.save(path)

def to_proto(instance: Any) -> bytes:
    instance_proto = InstanceProto()

    if type(instance) == bytes:
        instance_proto.type = InstanceTypeProto.INSTANCE_TYPE_PROTO_RAW
        instance_proto.raw_data = instance
    elif type(instance) == str:
        instance_proto.type = InstanceTypeProto.INSTANCE_TYPE_PROTO_TEXT
        instance_proto.text = instance
    elif type(instance) == np.ndarray:
        instance_proto.type = InstanceTypeProto.INSTANCE_TYPE_PROTO_TENSOR
        instance_proto.tensor.data = instance.tobytes()
        instance_proto.tensor.dtype = np.dtype(instance.dtype).str
        instance_proto.tensor.shape.extend(instance.shape)
    elif np.issubdtype(type(instance), np.integer):
        instance_proto.type = InstanceTypeProto.INSTANCE_TYPE_PROTO_DISCRETE
        instance_proto.discrete = int(instance)
    elif np.issubdtype(type(instance), np.floating):
        instance_proto.type = InstanceTypeProto.INSTANCE_TYPE_PROTO_FLOAT
        instance_proto.float = float(instance)
    elif np.issubdtype(type(instance), np.bool_):
        instance_proto.type = InstanceTypeProto.INSTANCE_TYPE_PROTO_BOOL
        instance_proto.boolean = bool(instance)
    elif instance is None:
        instance_proto.type = InstanceTypeProto.INSTANCE_TYPE_PROTO_NULL
    elif type(instance) == GraphInstance:
        instance_proto.type = InstanceTypeProto.INSTANCE_TYPE_PROTO_GRAPH
        instance_proto.graph.nodes.MergeFrom(to_proto(instance.nodes).tensor)
        instance_proto.graph.edges.MergeFrom(to_proto(instance.edges).tensor)
        instance_proto.graph.edge_links.MergeFrom(to_proto(instance.edge_links).tensor)
    elif isinstance(instance, Mapping):
        instance_proto.type = InstanceTypeProto.INSTANCE_TYPE_PROTO_DICT
        for key, value in instance.items():
            instance_proto.dict[key].MergeFrom(to_proto(value))
    elif isinstance(instance, Iterable):
        instance_proto.type = InstanceTypeProto.INSTANCE_TYPE_PROTO_LIST
        for value in instance:
            instance_proto.list.append(to_proto(value))
    
    return instance_proto