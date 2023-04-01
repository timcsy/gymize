// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: signaling.proto

#include "signaling.pb.h"

#include <algorithm>

#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/extension_set.h>
#include <google/protobuf/wire_format_lite.h>
#include <google/protobuf/descriptor.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/reflection_ops.h>
#include <google/protobuf/wire_format.h>
// @@protoc_insertion_point(includes)
#include <google/protobuf/port_def.inc>

PROTOBUF_PRAGMA_INIT_SEG
constexpr Signal::Signal(
  ::PROTOBUF_NAMESPACE_ID::internal::ConstantInitialized)
  : id_(&::PROTOBUF_NAMESPACE_ID::internal::fixed_address_empty_string)
  , data_(&::PROTOBUF_NAMESPACE_ID::internal::fixed_address_empty_string)
  , name_(&::PROTOBUF_NAMESPACE_ID::internal::fixed_address_empty_string)
  , url_(&::PROTOBUF_NAMESPACE_ID::internal::fixed_address_empty_string)
  , signal_type_(0)

  , peer_type_(0)
{}
struct SignalDefaultTypeInternal {
  constexpr SignalDefaultTypeInternal()
    : _instance(::PROTOBUF_NAMESPACE_ID::internal::ConstantInitialized{}) {}
  ~SignalDefaultTypeInternal() {}
  union {
    Signal _instance;
  };
};
PROTOBUF_ATTRIBUTE_NO_DESTROY PROTOBUF_CONSTINIT SignalDefaultTypeInternal _Signal_default_instance_;
static ::PROTOBUF_NAMESPACE_ID::Metadata file_level_metadata_signaling_2eproto[1];
static const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* file_level_enum_descriptors_signaling_2eproto[2];
static constexpr ::PROTOBUF_NAMESPACE_ID::ServiceDescriptor const** file_level_service_descriptors_signaling_2eproto = nullptr;

const uint32_t TableStruct_signaling_2eproto::offsets[] PROTOBUF_SECTION_VARIABLE(protodesc_cold) = {
  ~0u,  // no _has_bits_
  PROTOBUF_FIELD_OFFSET(::Signal, _internal_metadata_),
  ~0u,  // no _extensions_
  ~0u,  // no _oneof_case_
  ~0u,  // no _weak_field_map_
  ~0u,  // no _inlined_string_donated_
  PROTOBUF_FIELD_OFFSET(::Signal, signal_type_),
  PROTOBUF_FIELD_OFFSET(::Signal, id_),
  PROTOBUF_FIELD_OFFSET(::Signal, data_),
  PROTOBUF_FIELD_OFFSET(::Signal, name_),
  PROTOBUF_FIELD_OFFSET(::Signal, peer_type_),
  PROTOBUF_FIELD_OFFSET(::Signal, url_),
};
static const ::PROTOBUF_NAMESPACE_ID::internal::MigrationSchema schemas[] PROTOBUF_SECTION_VARIABLE(protodesc_cold) = {
  { 0, -1, -1, sizeof(::Signal)},
};

static ::PROTOBUF_NAMESPACE_ID::Message const * const file_default_instances[] = {
  reinterpret_cast<const ::PROTOBUF_NAMESPACE_ID::Message*>(&::_Signal_default_instance_),
};

const char descriptor_table_protodef_signaling_2eproto[] PROTOBUF_SECTION_VARIABLE(protodesc_cold) =
  "\n\017signaling.proto\"}\n\006Signal\022 \n\013signal_ty"
  "pe\030\001 \001(\0162\013.SignalType\022\n\n\002id\030\002 \001(\t\022\014\n\004dat"
  "a\030\003 \001(\014\022\014\n\004name\030\004 \001(\t\022\034\n\tpeer_type\030\005 \001(\016"
  "2\t.PeerType\022\013\n\003url\030\006 \001(\t*\206\001\n\nSignalType\022"
  "\033\n\027SIGNAL_TYPE_UNSPECIFIED\020\000\022\024\n\020SIGNAL_T"
  "YPE_INIT\020\001\022\026\n\022SIGNAL_TYPE_UPDATE\020\002\022\026\n\022SI"
  "GNAL_TYPE_RESUME\020\003\022\025\n\021SIGNAL_TYPE_CLOSE\020"
  "\004*R\n\010PeerType\022\031\n\025PEER_TYPE_UNSPECIFIED\020\000"
  "\022\024\n\020PEER_TYPE_ACTIVE\020\001\022\025\n\021PEER_TYPE_PASS"
  "IVE\020\002B\030\252\002\025PAIA.Gymize.Signalingb\006proto3"
  ;
static ::PROTOBUF_NAMESPACE_ID::internal::once_flag descriptor_table_signaling_2eproto_once;
const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable descriptor_table_signaling_2eproto = {
  false, false, 399, descriptor_table_protodef_signaling_2eproto, "signaling.proto", 
  &descriptor_table_signaling_2eproto_once, nullptr, 0, 1,
  schemas, file_default_instances, TableStruct_signaling_2eproto::offsets,
  file_level_metadata_signaling_2eproto, file_level_enum_descriptors_signaling_2eproto, file_level_service_descriptors_signaling_2eproto,
};
PROTOBUF_ATTRIBUTE_WEAK const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable* descriptor_table_signaling_2eproto_getter() {
  return &descriptor_table_signaling_2eproto;
}

// Force running AddDescriptors() at dynamic initialization time.
PROTOBUF_ATTRIBUTE_INIT_PRIORITY static ::PROTOBUF_NAMESPACE_ID::internal::AddDescriptorsRunner dynamic_init_dummy_signaling_2eproto(&descriptor_table_signaling_2eproto);
const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* SignalType_descriptor() {
  ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&descriptor_table_signaling_2eproto);
  return file_level_enum_descriptors_signaling_2eproto[0];
}
bool SignalType_IsValid(int value) {
  switch (value) {
    case 0:
    case 1:
    case 2:
    case 3:
    case 4:
      return true;
    default:
      return false;
  }
}

const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* PeerType_descriptor() {
  ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&descriptor_table_signaling_2eproto);
  return file_level_enum_descriptors_signaling_2eproto[1];
}
bool PeerType_IsValid(int value) {
  switch (value) {
    case 0:
    case 1:
    case 2:
      return true;
    default:
      return false;
  }
}


// ===================================================================

class Signal::_Internal {
 public:
};

Signal::Signal(::PROTOBUF_NAMESPACE_ID::Arena* arena,
                         bool is_message_owned)
  : ::PROTOBUF_NAMESPACE_ID::Message(arena, is_message_owned) {
  SharedCtor();
  if (!is_message_owned) {
    RegisterArenaDtor(arena);
  }
  // @@protoc_insertion_point(arena_constructor:Signal)
}
Signal::Signal(const Signal& from)
  : ::PROTOBUF_NAMESPACE_ID::Message() {
  _internal_metadata_.MergeFrom<::PROTOBUF_NAMESPACE_ID::UnknownFieldSet>(from._internal_metadata_);
  id_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  #ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
    id_.Set(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), "", GetArenaForAllocation());
  #endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
  if (!from._internal_id().empty()) {
    id_.Set(::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr::EmptyDefault{}, from._internal_id(), 
      GetArenaForAllocation());
  }
  data_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  #ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
    data_.Set(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), "", GetArenaForAllocation());
  #endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
  if (!from._internal_data().empty()) {
    data_.Set(::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr::EmptyDefault{}, from._internal_data(), 
      GetArenaForAllocation());
  }
  name_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  #ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
    name_.Set(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), "", GetArenaForAllocation());
  #endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
  if (!from._internal_name().empty()) {
    name_.Set(::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr::EmptyDefault{}, from._internal_name(), 
      GetArenaForAllocation());
  }
  url_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  #ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
    url_.Set(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), "", GetArenaForAllocation());
  #endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
  if (!from._internal_url().empty()) {
    url_.Set(::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr::EmptyDefault{}, from._internal_url(), 
      GetArenaForAllocation());
  }
  ::memcpy(&signal_type_, &from.signal_type_,
    static_cast<size_t>(reinterpret_cast<char*>(&peer_type_) -
    reinterpret_cast<char*>(&signal_type_)) + sizeof(peer_type_));
  // @@protoc_insertion_point(copy_constructor:Signal)
}

inline void Signal::SharedCtor() {
id_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
#ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
  id_.Set(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), "", GetArenaForAllocation());
#endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
data_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
#ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
  data_.Set(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), "", GetArenaForAllocation());
#endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
name_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
#ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
  name_.Set(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), "", GetArenaForAllocation());
#endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
url_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
#ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
  url_.Set(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), "", GetArenaForAllocation());
#endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
::memset(reinterpret_cast<char*>(this) + static_cast<size_t>(
    reinterpret_cast<char*>(&signal_type_) - reinterpret_cast<char*>(this)),
    0, static_cast<size_t>(reinterpret_cast<char*>(&peer_type_) -
    reinterpret_cast<char*>(&signal_type_)) + sizeof(peer_type_));
}

Signal::~Signal() {
  // @@protoc_insertion_point(destructor:Signal)
  if (GetArenaForAllocation() != nullptr) return;
  SharedDtor();
  _internal_metadata_.Delete<::PROTOBUF_NAMESPACE_ID::UnknownFieldSet>();
}

inline void Signal::SharedDtor() {
  GOOGLE_DCHECK(GetArenaForAllocation() == nullptr);
  id_.DestroyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  data_.DestroyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  name_.DestroyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  url_.DestroyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}

void Signal::ArenaDtor(void* object) {
  Signal* _this = reinterpret_cast< Signal* >(object);
  (void)_this;
}
void Signal::RegisterArenaDtor(::PROTOBUF_NAMESPACE_ID::Arena*) {
}
void Signal::SetCachedSize(int size) const {
  _cached_size_.Set(size);
}

void Signal::Clear() {
// @@protoc_insertion_point(message_clear_start:Signal)
  uint32_t cached_has_bits = 0;
  // Prevent compiler warnings about cached_has_bits being unused
  (void) cached_has_bits;

  id_.ClearToEmpty();
  data_.ClearToEmpty();
  name_.ClearToEmpty();
  url_.ClearToEmpty();
  ::memset(&signal_type_, 0, static_cast<size_t>(
      reinterpret_cast<char*>(&peer_type_) -
      reinterpret_cast<char*>(&signal_type_)) + sizeof(peer_type_));
  _internal_metadata_.Clear<::PROTOBUF_NAMESPACE_ID::UnknownFieldSet>();
}

const char* Signal::_InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) {
#define CHK_(x) if (PROTOBUF_PREDICT_FALSE(!(x))) goto failure
  while (!ctx->Done(&ptr)) {
    uint32_t tag;
    ptr = ::PROTOBUF_NAMESPACE_ID::internal::ReadTag(ptr, &tag);
    switch (tag >> 3) {
      // .SignalType signal_type = 1;
      case 1:
        if (PROTOBUF_PREDICT_TRUE(static_cast<uint8_t>(tag) == 8)) {
          uint64_t val = ::PROTOBUF_NAMESPACE_ID::internal::ReadVarint64(&ptr);
          CHK_(ptr);
          _internal_set_signal_type(static_cast<::SignalType>(val));
        } else
          goto handle_unusual;
        continue;
      // string id = 2;
      case 2:
        if (PROTOBUF_PREDICT_TRUE(static_cast<uint8_t>(tag) == 18)) {
          auto str = _internal_mutable_id();
          ptr = ::PROTOBUF_NAMESPACE_ID::internal::InlineGreedyStringParser(str, ptr, ctx);
          CHK_(::PROTOBUF_NAMESPACE_ID::internal::VerifyUTF8(str, "Signal.id"));
          CHK_(ptr);
        } else
          goto handle_unusual;
        continue;
      // bytes data = 3;
      case 3:
        if (PROTOBUF_PREDICT_TRUE(static_cast<uint8_t>(tag) == 26)) {
          auto str = _internal_mutable_data();
          ptr = ::PROTOBUF_NAMESPACE_ID::internal::InlineGreedyStringParser(str, ptr, ctx);
          CHK_(ptr);
        } else
          goto handle_unusual;
        continue;
      // string name = 4;
      case 4:
        if (PROTOBUF_PREDICT_TRUE(static_cast<uint8_t>(tag) == 34)) {
          auto str = _internal_mutable_name();
          ptr = ::PROTOBUF_NAMESPACE_ID::internal::InlineGreedyStringParser(str, ptr, ctx);
          CHK_(::PROTOBUF_NAMESPACE_ID::internal::VerifyUTF8(str, "Signal.name"));
          CHK_(ptr);
        } else
          goto handle_unusual;
        continue;
      // .PeerType peer_type = 5;
      case 5:
        if (PROTOBUF_PREDICT_TRUE(static_cast<uint8_t>(tag) == 40)) {
          uint64_t val = ::PROTOBUF_NAMESPACE_ID::internal::ReadVarint64(&ptr);
          CHK_(ptr);
          _internal_set_peer_type(static_cast<::PeerType>(val));
        } else
          goto handle_unusual;
        continue;
      // string url = 6;
      case 6:
        if (PROTOBUF_PREDICT_TRUE(static_cast<uint8_t>(tag) == 50)) {
          auto str = _internal_mutable_url();
          ptr = ::PROTOBUF_NAMESPACE_ID::internal::InlineGreedyStringParser(str, ptr, ctx);
          CHK_(::PROTOBUF_NAMESPACE_ID::internal::VerifyUTF8(str, "Signal.url"));
          CHK_(ptr);
        } else
          goto handle_unusual;
        continue;
      default:
        goto handle_unusual;
    }  // switch
  handle_unusual:
    if ((tag == 0) || ((tag & 7) == 4)) {
      CHK_(ptr);
      ctx->SetLastTag(tag);
      goto message_done;
    }
    ptr = UnknownFieldParse(
        tag,
        _internal_metadata_.mutable_unknown_fields<::PROTOBUF_NAMESPACE_ID::UnknownFieldSet>(),
        ptr, ctx);
    CHK_(ptr != nullptr);
  }  // while
message_done:
  return ptr;
failure:
  ptr = nullptr;
  goto message_done;
#undef CHK_
}

uint8_t* Signal::_InternalSerialize(
    uint8_t* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const {
  // @@protoc_insertion_point(serialize_to_array_start:Signal)
  uint32_t cached_has_bits = 0;
  (void) cached_has_bits;

  // .SignalType signal_type = 1;
  if (this->_internal_signal_type() != 0) {
    target = stream->EnsureSpace(target);
    target = ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteEnumToArray(
      1, this->_internal_signal_type(), target);
  }

  // string id = 2;
  if (!this->_internal_id().empty()) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
      this->_internal_id().data(), static_cast<int>(this->_internal_id().length()),
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::SERIALIZE,
      "Signal.id");
    target = stream->WriteStringMaybeAliased(
        2, this->_internal_id(), target);
  }

  // bytes data = 3;
  if (!this->_internal_data().empty()) {
    target = stream->WriteBytesMaybeAliased(
        3, this->_internal_data(), target);
  }

  // string name = 4;
  if (!this->_internal_name().empty()) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
      this->_internal_name().data(), static_cast<int>(this->_internal_name().length()),
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::SERIALIZE,
      "Signal.name");
    target = stream->WriteStringMaybeAliased(
        4, this->_internal_name(), target);
  }

  // .PeerType peer_type = 5;
  if (this->_internal_peer_type() != 0) {
    target = stream->EnsureSpace(target);
    target = ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteEnumToArray(
      5, this->_internal_peer_type(), target);
  }

  // string url = 6;
  if (!this->_internal_url().empty()) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
      this->_internal_url().data(), static_cast<int>(this->_internal_url().length()),
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::SERIALIZE,
      "Signal.url");
    target = stream->WriteStringMaybeAliased(
        6, this->_internal_url(), target);
  }

  if (PROTOBUF_PREDICT_FALSE(_internal_metadata_.have_unknown_fields())) {
    target = ::PROTOBUF_NAMESPACE_ID::internal::WireFormat::InternalSerializeUnknownFieldsToArray(
        _internal_metadata_.unknown_fields<::PROTOBUF_NAMESPACE_ID::UnknownFieldSet>(::PROTOBUF_NAMESPACE_ID::UnknownFieldSet::default_instance), target, stream);
  }
  // @@protoc_insertion_point(serialize_to_array_end:Signal)
  return target;
}

size_t Signal::ByteSizeLong() const {
// @@protoc_insertion_point(message_byte_size_start:Signal)
  size_t total_size = 0;

  uint32_t cached_has_bits = 0;
  // Prevent compiler warnings about cached_has_bits being unused
  (void) cached_has_bits;

  // string id = 2;
  if (!this->_internal_id().empty()) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::StringSize(
        this->_internal_id());
  }

  // bytes data = 3;
  if (!this->_internal_data().empty()) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::BytesSize(
        this->_internal_data());
  }

  // string name = 4;
  if (!this->_internal_name().empty()) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::StringSize(
        this->_internal_name());
  }

  // string url = 6;
  if (!this->_internal_url().empty()) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::StringSize(
        this->_internal_url());
  }

  // .SignalType signal_type = 1;
  if (this->_internal_signal_type() != 0) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::EnumSize(this->_internal_signal_type());
  }

  // .PeerType peer_type = 5;
  if (this->_internal_peer_type() != 0) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::EnumSize(this->_internal_peer_type());
  }

  return MaybeComputeUnknownFieldsSize(total_size, &_cached_size_);
}

const ::PROTOBUF_NAMESPACE_ID::Message::ClassData Signal::_class_data_ = {
    ::PROTOBUF_NAMESPACE_ID::Message::CopyWithSizeCheck,
    Signal::MergeImpl
};
const ::PROTOBUF_NAMESPACE_ID::Message::ClassData*Signal::GetClassData() const { return &_class_data_; }

void Signal::MergeImpl(::PROTOBUF_NAMESPACE_ID::Message* to,
                      const ::PROTOBUF_NAMESPACE_ID::Message& from) {
  static_cast<Signal *>(to)->MergeFrom(
      static_cast<const Signal &>(from));
}


void Signal::MergeFrom(const Signal& from) {
// @@protoc_insertion_point(class_specific_merge_from_start:Signal)
  GOOGLE_DCHECK_NE(&from, this);
  uint32_t cached_has_bits = 0;
  (void) cached_has_bits;

  if (!from._internal_id().empty()) {
    _internal_set_id(from._internal_id());
  }
  if (!from._internal_data().empty()) {
    _internal_set_data(from._internal_data());
  }
  if (!from._internal_name().empty()) {
    _internal_set_name(from._internal_name());
  }
  if (!from._internal_url().empty()) {
    _internal_set_url(from._internal_url());
  }
  if (from._internal_signal_type() != 0) {
    _internal_set_signal_type(from._internal_signal_type());
  }
  if (from._internal_peer_type() != 0) {
    _internal_set_peer_type(from._internal_peer_type());
  }
  _internal_metadata_.MergeFrom<::PROTOBUF_NAMESPACE_ID::UnknownFieldSet>(from._internal_metadata_);
}

void Signal::CopyFrom(const Signal& from) {
// @@protoc_insertion_point(class_specific_copy_from_start:Signal)
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

bool Signal::IsInitialized() const {
  return true;
}

void Signal::InternalSwap(Signal* other) {
  using std::swap;
  auto* lhs_arena = GetArenaForAllocation();
  auto* rhs_arena = other->GetArenaForAllocation();
  _internal_metadata_.InternalSwap(&other->_internal_metadata_);
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr::InternalSwap(
      &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      &id_, lhs_arena,
      &other->id_, rhs_arena
  );
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr::InternalSwap(
      &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      &data_, lhs_arena,
      &other->data_, rhs_arena
  );
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr::InternalSwap(
      &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      &name_, lhs_arena,
      &other->name_, rhs_arena
  );
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr::InternalSwap(
      &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      &url_, lhs_arena,
      &other->url_, rhs_arena
  );
  ::PROTOBUF_NAMESPACE_ID::internal::memswap<
      PROTOBUF_FIELD_OFFSET(Signal, peer_type_)
      + sizeof(Signal::peer_type_)
      - PROTOBUF_FIELD_OFFSET(Signal, signal_type_)>(
          reinterpret_cast<char*>(&signal_type_),
          reinterpret_cast<char*>(&other->signal_type_));
}

::PROTOBUF_NAMESPACE_ID::Metadata Signal::GetMetadata() const {
  return ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(
      &descriptor_table_signaling_2eproto_getter, &descriptor_table_signaling_2eproto_once,
      file_level_metadata_signaling_2eproto[0]);
}

// @@protoc_insertion_point(namespace_scope)
PROTOBUF_NAMESPACE_OPEN
template<> PROTOBUF_NOINLINE ::Signal* Arena::CreateMaybeMessage< ::Signal >(Arena* arena) {
  return Arena::CreateMessageInternal< ::Signal >(arena);
}
PROTOBUF_NAMESPACE_CLOSE

// @@protoc_insertion_point(global_scope)
#include <google/protobuf/port_undef.inc>
