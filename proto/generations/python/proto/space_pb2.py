# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: space.proto
"""Generated protocol buffer code."""
from google.protobuf.internal import enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x0bspace.proto\"\xd4\x01\n\x06Tensor\x12\r\n\x05shape\x18\x01 \x03(\x05\x12\x1c\n\tdata_type\x18\x02 \x01(\x0e\x32\t.DataType\x12\x13\n\x0b\x66loat_array\x18\x03 \x03(\x02\x12\x14\n\x0c\x64ouble_array\x18\x04 \x03(\x01\x12\x11\n\tint_array\x18\x05 \x03(\x11\x12\x12\n\nlong_array\x18\x06 \x03(\x12\x12\x1a\n\x12unsigned_int_array\x18\x07 \x03(\r\x12\x1b\n\x13unsigned_long_array\x18\x08 \x03(\x04\x12\x12\n\nbool_array\x18\t \x03(\x08\"D\n\nGraphSpace\x12\x1a\n\nnode_space\x18\x01 \x01(\x0b\x32\x06.Space\x12\x1a\n\nedge_space\x18\x02 \x01(\x0b\x32\x06.Space\"T\n\x05Graph\x12\x16\n\x05nodes\x18\x01 \x01(\x0b\x32\x07.Tensor\x12\x16\n\x05\x65\x64ges\x18\x02 \x01(\x0b\x32\x07.Tensor\x12\x1b\n\nedge_links\x18\x03 \x01(\x0b\x32\x07.Tensor\"k\n\x05Image\x12*\n\x10\x63ompression_type\x18\x01 \x01(\x0e\x32\x10.CompressionType\x12\x0c\n\x04\x64\x61ta\x18\x02 \x01(\x0c\x12\r\n\x05shape\x18\x03 \x03(\x05\x12\x19\n\x11\x64imension_mapping\x18\x04 \x03(\x05\"\x9e\x03\n\x05Space\x12\x1e\n\nspace_type\x18\x01 \x01(\x0e\x32\n.SpaceType\x12\x13\n\x0b\x64\x65scription\x18\x02 \x01(\t\x12\r\n\x05shape\x18\x03 \x03(\x05\x12\x11\n\tdata_type\x18\x04 \x01(\t\x12\x0b\n\x03low\x18\x05 \x03(\x02\x12\x0c\n\x04high\x18\x06 \x03(\x02\x12\x0b\n\x03min\x18\x07 \x01(\x05\x12\x0b\n\x03max\x18\x08 \x01(\x05\x12\x0c\n\x04nvec\x18\t \x03(\x05\x12\x0f\n\x07\x63harset\x18\n \x03(\t\x12)\n\ndict_space\x18\x0b \x03(\x0b\x32\x15.Space.DictSpaceEntry\x12)\n\nlist_space\x18\x0c \x03(\x0b\x32\x15.Space.ListSpaceEntry\x12 \n\x0bgraph_space\x18\r \x01(\x0b\x32\x0b.GraphSpace\x1a\x38\n\x0e\x44ictSpaceEntry\x12\x0b\n\x03key\x18\x01 \x01(\t\x12\x15\n\x05value\x18\x02 \x01(\x0b\x32\x06.Space:\x02\x38\x01\x1a\x38\n\x0eListSpaceEntry\x12\x0b\n\x03key\x18\x01 \x01(\x05\x12\x15\n\x05value\x18\x02 \x01(\x0b\x32\x06.Space:\x02\x38\x01\"\x95\x03\n\x04\x44\x61ta\x12\x1e\n\nspace_type\x18\x01 \x01(\x0e\x32\n.SpaceType\x12\x11\n\tdata_type\x18\x02 \x01(\t\x12\x10\n\x08raw_data\x18\x03 \x01(\x0c\x12\x14\n\x03\x62ox\x18\x04 \x01(\x0b\x32\x07.Tensor\x12\x10\n\x08\x64iscrete\x18\x05 \x01(\x05\x12\x1d\n\x0cmulti_binary\x18\x06 \x01(\x0b\x32\x07.Tensor\x12\x1f\n\x0emulti_discrete\x18\x07 \x01(\x0b\x32\x07.Tensor\x12\x0c\n\x04text\x18\x08 \x01(\t\x12\x1d\n\x04\x64ict\x18\t \x03(\x0b\x32\x0f.Data.DictEntry\x12\x1d\n\x04list\x18\n \x03(\x0b\x32\x0f.Data.ListEntry\x12\x15\n\x05graph\x18\x0b \x01(\x0b\x32\x06.Graph\x12\x15\n\x05image\x18\x0c \x01(\x0b\x32\x06.Image\x1a\x32\n\tDictEntry\x12\x0b\n\x03key\x18\x01 \x01(\t\x12\x14\n\x05value\x18\x02 \x01(\x0b\x32\x05.Data:\x02\x38\x01\x1a\x32\n\tListEntry\x12\x0b\n\x03key\x18\x01 \x01(\x05\x12\x14\n\x05value\x18\x02 \x01(\x0b\x32\x05.Data:\x02\x38\x01*\xb4\x01\n\x08\x44\x61taType\x12\x19\n\x15\x44\x41TA_TYPE_UNSPECIFIED\x10\x00\x12\x13\n\x0f\x44\x41TA_TYPE_FLOAT\x10\x01\x12\x14\n\x10\x44\x41TA_TYPE_DOUBLE\x10\x02\x12\x11\n\rDATA_TYPE_INT\x10\x03\x12\x12\n\x0e\x44\x41TA_TYPE_LONG\x10\x04\x12\x12\n\x0e\x44\x41TA_TYPE_UINT\x10\x05\x12\x13\n\x0f\x44\x41TA_TYPE_ULONG\x10\x06\x12\x12\n\x0e\x44\x41TA_TYPE_BOOL\x10\x07*\x82\x01\n\x0f\x43ompressionType\x12 \n\x1c\x43OMPRESSION_TYPE_UNSPECIFIED\x10\x00\x12\x19\n\x15\x43OMPRESSION_TYPE_NONE\x10\x01\x12\x18\n\x14\x43OMPRESSION_TYPE_PNG\x10\x02\x12\x18\n\x14\x43OMPRESSION_TYPE_JPG\x10\x03*\xa9\x02\n\tSpaceType\x12\x1a\n\x16SPACE_TYPE_UNSPECIFIED\x10\x00\x12\x12\n\x0eSPACE_TYPE_RAW\x10\x01\x12\x12\n\x0eSPACE_TYPE_BOX\x10\x02\x12\x17\n\x13SPACE_TYPE_DISCRETE\x10\x03\x12\x1b\n\x17SPACE_TYPE_MULTI_BINARY\x10\x04\x12\x1d\n\x19SPACE_TYPE_MULTI_DISCRETE\x10\x05\x12\x13\n\x0fSPACE_TYPE_TEXT\x10\x06\x12\x13\n\x0fSPACE_TYPE_DICT\x10\x07\x12\x14\n\x10SPACE_TYPE_TUPLE\x10\x08\x12\x17\n\x13SPACE_TYPE_SEQUENCE\x10\t\x12\x14\n\x10SPACE_TYPE_GRAPH\x10\n\x12\x14\n\x10SPACE_TYPE_IMAGE\x10\x0b\x42\x17\xaa\x02\x14PAIA.Marenv.Protobufb\x06proto3')

_DATATYPE = DESCRIPTOR.enum_types_by_name['DataType']
DataType = enum_type_wrapper.EnumTypeWrapper(_DATATYPE)
_COMPRESSIONTYPE = DESCRIPTOR.enum_types_by_name['CompressionType']
CompressionType = enum_type_wrapper.EnumTypeWrapper(_COMPRESSIONTYPE)
_SPACETYPE = DESCRIPTOR.enum_types_by_name['SpaceType']
SpaceType = enum_type_wrapper.EnumTypeWrapper(_SPACETYPE)
DATA_TYPE_UNSPECIFIED = 0
DATA_TYPE_FLOAT = 1
DATA_TYPE_DOUBLE = 2
DATA_TYPE_INT = 3
DATA_TYPE_LONG = 4
DATA_TYPE_UINT = 5
DATA_TYPE_ULONG = 6
DATA_TYPE_BOOL = 7
COMPRESSION_TYPE_UNSPECIFIED = 0
COMPRESSION_TYPE_NONE = 1
COMPRESSION_TYPE_PNG = 2
COMPRESSION_TYPE_JPG = 3
SPACE_TYPE_UNSPECIFIED = 0
SPACE_TYPE_RAW = 1
SPACE_TYPE_BOX = 2
SPACE_TYPE_DISCRETE = 3
SPACE_TYPE_MULTI_BINARY = 4
SPACE_TYPE_MULTI_DISCRETE = 5
SPACE_TYPE_TEXT = 6
SPACE_TYPE_DICT = 7
SPACE_TYPE_TUPLE = 8
SPACE_TYPE_SEQUENCE = 9
SPACE_TYPE_GRAPH = 10
SPACE_TYPE_IMAGE = 11


_TENSOR = DESCRIPTOR.message_types_by_name['Tensor']
_GRAPHSPACE = DESCRIPTOR.message_types_by_name['GraphSpace']
_GRAPH = DESCRIPTOR.message_types_by_name['Graph']
_IMAGE = DESCRIPTOR.message_types_by_name['Image']
_SPACE = DESCRIPTOR.message_types_by_name['Space']
_SPACE_DICTSPACEENTRY = _SPACE.nested_types_by_name['DictSpaceEntry']
_SPACE_LISTSPACEENTRY = _SPACE.nested_types_by_name['ListSpaceEntry']
_DATA = DESCRIPTOR.message_types_by_name['Data']
_DATA_DICTENTRY = _DATA.nested_types_by_name['DictEntry']
_DATA_LISTENTRY = _DATA.nested_types_by_name['ListEntry']
Tensor = _reflection.GeneratedProtocolMessageType('Tensor', (_message.Message,), {
  'DESCRIPTOR' : _TENSOR,
  '__module__' : 'space_pb2'
  # @@protoc_insertion_point(class_scope:Tensor)
  })
_sym_db.RegisterMessage(Tensor)

GraphSpace = _reflection.GeneratedProtocolMessageType('GraphSpace', (_message.Message,), {
  'DESCRIPTOR' : _GRAPHSPACE,
  '__module__' : 'space_pb2'
  # @@protoc_insertion_point(class_scope:GraphSpace)
  })
_sym_db.RegisterMessage(GraphSpace)

Graph = _reflection.GeneratedProtocolMessageType('Graph', (_message.Message,), {
  'DESCRIPTOR' : _GRAPH,
  '__module__' : 'space_pb2'
  # @@protoc_insertion_point(class_scope:Graph)
  })
_sym_db.RegisterMessage(Graph)

Image = _reflection.GeneratedProtocolMessageType('Image', (_message.Message,), {
  'DESCRIPTOR' : _IMAGE,
  '__module__' : 'space_pb2'
  # @@protoc_insertion_point(class_scope:Image)
  })
_sym_db.RegisterMessage(Image)

Space = _reflection.GeneratedProtocolMessageType('Space', (_message.Message,), {

  'DictSpaceEntry' : _reflection.GeneratedProtocolMessageType('DictSpaceEntry', (_message.Message,), {
    'DESCRIPTOR' : _SPACE_DICTSPACEENTRY,
    '__module__' : 'space_pb2'
    # @@protoc_insertion_point(class_scope:Space.DictSpaceEntry)
    })
  ,

  'ListSpaceEntry' : _reflection.GeneratedProtocolMessageType('ListSpaceEntry', (_message.Message,), {
    'DESCRIPTOR' : _SPACE_LISTSPACEENTRY,
    '__module__' : 'space_pb2'
    # @@protoc_insertion_point(class_scope:Space.ListSpaceEntry)
    })
  ,
  'DESCRIPTOR' : _SPACE,
  '__module__' : 'space_pb2'
  # @@protoc_insertion_point(class_scope:Space)
  })
_sym_db.RegisterMessage(Space)
_sym_db.RegisterMessage(Space.DictSpaceEntry)
_sym_db.RegisterMessage(Space.ListSpaceEntry)

Data = _reflection.GeneratedProtocolMessageType('Data', (_message.Message,), {

  'DictEntry' : _reflection.GeneratedProtocolMessageType('DictEntry', (_message.Message,), {
    'DESCRIPTOR' : _DATA_DICTENTRY,
    '__module__' : 'space_pb2'
    # @@protoc_insertion_point(class_scope:Data.DictEntry)
    })
  ,

  'ListEntry' : _reflection.GeneratedProtocolMessageType('ListEntry', (_message.Message,), {
    'DESCRIPTOR' : _DATA_LISTENTRY,
    '__module__' : 'space_pb2'
    # @@protoc_insertion_point(class_scope:Data.ListEntry)
    })
  ,
  'DESCRIPTOR' : _DATA,
  '__module__' : 'space_pb2'
  # @@protoc_insertion_point(class_scope:Data)
  })
_sym_db.RegisterMessage(Data)
_sym_db.RegisterMessage(Data.DictEntry)
_sym_db.RegisterMessage(Data.ListEntry)

if _descriptor._USE_C_DESCRIPTORS == False:

  DESCRIPTOR._options = None
  DESCRIPTOR._serialized_options = b'\252\002\024PAIA.Marenv.Protobuf'
  _SPACE_DICTSPACEENTRY._options = None
  _SPACE_DICTSPACEENTRY._serialized_options = b'8\001'
  _SPACE_LISTSPACEENTRY._options = None
  _SPACE_LISTSPACEENTRY._serialized_options = b'8\001'
  _DATA_DICTENTRY._options = None
  _DATA_DICTENTRY._serialized_options = b'8\001'
  _DATA_LISTENTRY._options = None
  _DATA_LISTENTRY._serialized_options = b'8\001'
  _DATATYPE._serialized_start=1321
  _DATATYPE._serialized_end=1501
  _COMPRESSIONTYPE._serialized_start=1504
  _COMPRESSIONTYPE._serialized_end=1634
  _SPACETYPE._serialized_start=1637
  _SPACETYPE._serialized_end=1934
  _TENSOR._serialized_start=16
  _TENSOR._serialized_end=228
  _GRAPHSPACE._serialized_start=230
  _GRAPHSPACE._serialized_end=298
  _GRAPH._serialized_start=300
  _GRAPH._serialized_end=384
  _IMAGE._serialized_start=386
  _IMAGE._serialized_end=493
  _SPACE._serialized_start=496
  _SPACE._serialized_end=910
  _SPACE_DICTSPACEENTRY._serialized_start=796
  _SPACE_DICTSPACEENTRY._serialized_end=852
  _SPACE_LISTSPACEENTRY._serialized_start=854
  _SPACE_LISTSPACEENTRY._serialized_end=910
  _DATA._serialized_start=913
  _DATA._serialized_end=1318
  _DATA_DICTENTRY._serialized_start=1216
  _DATA_DICTENTRY._serialized_end=1266
  _DATA_LISTENTRY._serialized_start=1268
  _DATA_LISTENTRY._serialized_end=1318
# @@protoc_insertion_point(module_scope)
