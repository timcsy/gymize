# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: signaling.proto
"""Generated protocol buffer code."""
from google.protobuf.internal import enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x0fsignaling.proto\"}\n\x06Signal\x12 \n\x0bsignal_type\x18\x01 \x01(\x0e\x32\x0b.SignalType\x12\n\n\x02id\x18\x02 \x01(\t\x12\x0c\n\x04\x64\x61ta\x18\x03 \x01(\x0c\x12\x0c\n\x04name\x18\x04 \x01(\t\x12\x1c\n\tpeer_type\x18\x05 \x01(\x0e\x32\t.PeerType\x12\x0b\n\x03url\x18\x06 \x01(\t*\x86\x01\n\nSignalType\x12\x1b\n\x17SIGNAL_TYPE_UNSPECIFIED\x10\x00\x12\x14\n\x10SIGNAL_TYPE_INIT\x10\x01\x12\x16\n\x12SIGNAL_TYPE_UPDATE\x10\x02\x12\x16\n\x12SIGNAL_TYPE_RESUME\x10\x03\x12\x15\n\x11SIGNAL_TYPE_CLOSE\x10\x04*R\n\x08PeerType\x12\x19\n\x15PEER_TYPE_UNSPECIFIED\x10\x00\x12\x14\n\x10PEER_TYPE_ACTIVE\x10\x01\x12\x15\n\x11PEER_TYPE_PASSIVE\x10\x02\x42\x18\xaa\x02\x15PAIA.Marenv.Signalingb\x06proto3')

_SIGNALTYPE = DESCRIPTOR.enum_types_by_name['SignalType']
SignalType = enum_type_wrapper.EnumTypeWrapper(_SIGNALTYPE)
_PEERTYPE = DESCRIPTOR.enum_types_by_name['PeerType']
PeerType = enum_type_wrapper.EnumTypeWrapper(_PEERTYPE)
SIGNAL_TYPE_UNSPECIFIED = 0
SIGNAL_TYPE_INIT = 1
SIGNAL_TYPE_UPDATE = 2
SIGNAL_TYPE_RESUME = 3
SIGNAL_TYPE_CLOSE = 4
PEER_TYPE_UNSPECIFIED = 0
PEER_TYPE_ACTIVE = 1
PEER_TYPE_PASSIVE = 2


_SIGNAL = DESCRIPTOR.message_types_by_name['Signal']
Signal = _reflection.GeneratedProtocolMessageType('Signal', (_message.Message,), {
  'DESCRIPTOR' : _SIGNAL,
  '__module__' : 'signaling_pb2'
  # @@protoc_insertion_point(class_scope:Signal)
  })
_sym_db.RegisterMessage(Signal)

if _descriptor._USE_C_DESCRIPTORS == False:

  DESCRIPTOR._options = None
  DESCRIPTOR._serialized_options = b'\252\002\025PAIA.Marenv.Signaling'
  _SIGNALTYPE._serialized_start=147
  _SIGNALTYPE._serialized_end=281
  _PEERTYPE._serialized_start=283
  _PEERTYPE._serialized_end=365
  _SIGNAL._serialized_start=19
  _SIGNAL._serialized_end=144
# @@protoc_insertion_point(module_scope)
