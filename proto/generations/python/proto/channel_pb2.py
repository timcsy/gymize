# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: channel.proto
"""Generated protocol buffer code."""
from google.protobuf.internal import enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\rchannel.proto\"F\n\x06Header\x12\"\n\x0cmessage_type\x18\x01 \x01(\x0e\x32\x0c.MessageType\x12\n\n\x02id\x18\x02 \x01(\t\x12\x0c\n\x04uuid\x18\x03 \x01(\x0c\"0\n\x07\x43ontent\x12\r\n\x03raw\x18\x01 \x01(\x0cH\x00\x12\x0e\n\x04text\x18\x02 \x01(\tH\x00\x42\x06\n\x04\x64\x61ta\"=\n\x07Message\x12\x17\n\x06header\x18\x01 \x01(\x0b\x32\x07.Header\x12\x19\n\x07\x63ontent\x18\x02 \x01(\x0b\x32\x08.Content*z\n\x0bMessageType\x12\x1c\n\x18MESSAGE_TYPE_UNSPECIFIED\x10\x00\x12\x18\n\x14MESSAGE_TYPE_MESSAGE\x10\x01\x12\x18\n\x14MESSAGE_TYPE_REQUEST\x10\x02\x12\x19\n\x15MESSAGE_TYPE_RESPONSE\x10\x03\x42\x16\xaa\x02\x13PAIA.Gymize.Channelb\x06proto3')

_MESSAGETYPE = DESCRIPTOR.enum_types_by_name['MessageType']
MessageType = enum_type_wrapper.EnumTypeWrapper(_MESSAGETYPE)
MESSAGE_TYPE_UNSPECIFIED = 0
MESSAGE_TYPE_MESSAGE = 1
MESSAGE_TYPE_REQUEST = 2
MESSAGE_TYPE_RESPONSE = 3


_HEADER = DESCRIPTOR.message_types_by_name['Header']
_CONTENT = DESCRIPTOR.message_types_by_name['Content']
_MESSAGE = DESCRIPTOR.message_types_by_name['Message']
Header = _reflection.GeneratedProtocolMessageType('Header', (_message.Message,), {
  'DESCRIPTOR' : _HEADER,
  '__module__' : 'channel_pb2'
  # @@protoc_insertion_point(class_scope:Header)
  })
_sym_db.RegisterMessage(Header)

Content = _reflection.GeneratedProtocolMessageType('Content', (_message.Message,), {
  'DESCRIPTOR' : _CONTENT,
  '__module__' : 'channel_pb2'
  # @@protoc_insertion_point(class_scope:Content)
  })
_sym_db.RegisterMessage(Content)

Message = _reflection.GeneratedProtocolMessageType('Message', (_message.Message,), {
  'DESCRIPTOR' : _MESSAGE,
  '__module__' : 'channel_pb2'
  # @@protoc_insertion_point(class_scope:Message)
  })
_sym_db.RegisterMessage(Message)

if _descriptor._USE_C_DESCRIPTORS == False:

  DESCRIPTOR._options = None
  DESCRIPTOR._serialized_options = b'\252\002\023PAIA.Gymize.Channel'
  _MESSAGETYPE._serialized_start=202
  _MESSAGETYPE._serialized_end=324
  _HEADER._serialized_start=17
  _HEADER._serialized_end=87
  _CONTENT._serialized_start=89
  _CONTENT._serialized_end=137
  _MESSAGE._serialized_start=139
  _MESSAGE._serialized_end=200
# @@protoc_insertion_point(module_scope)
