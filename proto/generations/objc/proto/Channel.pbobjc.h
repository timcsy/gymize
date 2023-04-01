// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: channel.proto

// This CPP symbol can be defined to use imports that match up to the framework
// imports needed when using CocoaPods.
#if !defined(GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS)
 #define GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS 0
#endif

#if GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS
 #import <Protobuf/GPBProtocolBuffers.h>
#else
 #import "GPBProtocolBuffers.h"
#endif

#if GOOGLE_PROTOBUF_OBJC_VERSION < 30004
#error This file was generated by a newer version of protoc which is incompatible with your Protocol Buffer library sources.
#endif
#if 30004 < GOOGLE_PROTOBUF_OBJC_MIN_SUPPORTED_VERSION
#error This file was generated by an older version of protoc which is incompatible with your Protocol Buffer library sources.
#endif

// @@protoc_insertion_point(imports)

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"

CF_EXTERN_C_BEGIN

@class Content;
@class Header;

NS_ASSUME_NONNULL_BEGIN

#pragma mark - Enum MessageType

typedef GPB_ENUM(MessageType) {
  /**
   * Value used if any message's field encounters a value that is not defined
   * by this enum. The message will also have C functions to get/set the rawValue
   * of the field.
   **/
  MessageType_GPBUnrecognizedEnumeratorValue = kGPBUnrecognizedEnumeratorValue,
  MessageType_MessageTypeUnspecified = 0,
  MessageType_MessageTypeMessage = 1,
  MessageType_MessageTypeRequest = 2,
  MessageType_MessageTypeResponse = 3,
};

GPBEnumDescriptor *MessageType_EnumDescriptor(void);

/**
 * Checks to see if the given value is defined by the enum or was not known at
 * the time this source was generated.
 **/
BOOL MessageType_IsValidValue(int32_t value);

#pragma mark - ChannelRoot

/**
 * Exposes the extension registry for this file.
 *
 * The base class provides:
 * @code
 *   + (GPBExtensionRegistry *)extensionRegistry;
 * @endcode
 * which is a @c GPBExtensionRegistry that includes all the extensions defined by
 * this file and all files that it depends on.
 **/
GPB_FINAL @interface ChannelRoot : GPBRootObject
@end

#pragma mark - Header

typedef GPB_ENUM(Header_FieldNumber) {
  Header_FieldNumber_MessageType = 1,
  Header_FieldNumber_Id_p = 2,
  Header_FieldNumber_Uuid = 3,
};

GPB_FINAL @interface Header : GPBMessage

@property(nonatomic, readwrite) MessageType messageType;

@property(nonatomic, readwrite, copy, null_resettable) NSString *id_p;

@property(nonatomic, readwrite, copy, null_resettable) NSData *uuid;

@end

/**
 * Fetches the raw value of a @c Header's @c messageType property, even
 * if the value was not defined by the enum at the time the code was generated.
 **/
int32_t Header_MessageType_RawValue(Header *message);
/**
 * Sets the raw value of an @c Header's @c messageType property, allowing
 * it to be set to a value that was not defined by the enum at the time the code
 * was generated.
 **/
void SetHeader_MessageType_RawValue(Header *message, int32_t value);

#pragma mark - Content

typedef GPB_ENUM(Content_FieldNumber) {
  Content_FieldNumber_Raw = 1,
  Content_FieldNumber_Text = 2,
};

typedef GPB_ENUM(Content_Data_OneOfCase) {
  Content_Data_OneOfCase_GPBUnsetOneOfCase = 0,
  Content_Data_OneOfCase_Raw = 1,
  Content_Data_OneOfCase_Text = 2,
};

GPB_FINAL @interface Content : GPBMessage

@property(nonatomic, readonly) Content_Data_OneOfCase dataOneOfCase;

@property(nonatomic, readwrite, copy, null_resettable) NSData *raw;

@property(nonatomic, readwrite, copy, null_resettable) NSString *text;

@end

/**
 * Clears whatever value was set for the oneof 'data'.
 **/
void Content_ClearDataOneOfCase(Content *message);

#pragma mark - Message

typedef GPB_ENUM(Message_FieldNumber) {
  Message_FieldNumber_Header = 1,
  Message_FieldNumber_Content = 2,
};

GPB_FINAL @interface Message : GPBMessage

@property(nonatomic, readwrite, strong, null_resettable) Header *header;
/** Test to see if @c header has been set. */
@property(nonatomic, readwrite) BOOL hasHeader;

@property(nonatomic, readwrite, strong, null_resettable) Content *content;
/** Test to see if @c content has been set. */
@property(nonatomic, readwrite) BOOL hasContent;

@end

NS_ASSUME_NONNULL_END

CF_EXTERN_C_END

#pragma clang diagnostic pop

// @@protoc_insertion_point(global_scope)