// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: gymize.proto

// This CPP symbol can be defined to use imports that match up to the framework
// imports needed when using CocoaPods.
#if !defined(GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS)
 #define GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS 0
#endif

#if GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS
 #import <Protobuf/GPBProtocolBuffers_RuntimeSupport.h>
#else
 #import "GPBProtocolBuffers_RuntimeSupport.h"
#endif

#import "Gymize.pbobjc.h"
#import "Space.pbobjc.h"
#import "Locator.pbobjc.h"
#import "Recording.pbobjc.h"
// @@protoc_insertion_point(imports)

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
#pragma clang diagnostic ignored "-Wdollar-in-identifier-extension"

#pragma mark - Objective C Class declarations
// Forward declarations of Objective C classes that we can use as
// static values in struct initializers.
// We don't use [Foo class] because it is not a static value.
GPBObjCClassDeclaration(ActionProto);
GPBObjCClassDeclaration(InfoProto);
GPBObjCClassDeclaration(InstanceProto);
GPBObjCClassDeclaration(LocatorProto);
GPBObjCClassDeclaration(ObservationProto);
GPBObjCClassDeclaration(RecordingProto);
GPBObjCClassDeclaration(RewardProto);

#pragma mark - GymizeRoot

@implementation GymizeRoot

// No extensions in the file and none of the imports (direct or indirect)
// defined extensions, so no need to generate +extensionRegistry.

@end

#pragma mark - GymizeRoot_FileDescriptor

static GPBFileDescriptor *GymizeRoot_FileDescriptor(void) {
  // This is called by +initialize so there is no need to worry
  // about thread safety of the singleton.
  static GPBFileDescriptor *descriptor = NULL;
  if (!descriptor) {
    GPB_DEBUG_CHECK_RUNTIME_VERSIONS();
    descriptor = [[GPBFileDescriptor alloc] initWithPackage:@""
                                                     syntax:GPBFileSyntaxProto3];
  }
  return descriptor;
}

#pragma mark - ActionProto

@implementation ActionProto

@dynamic agent;
@dynamic hasAction, action;

typedef struct ActionProto__storage_ {
  uint32_t _has_storage_[1];
  NSString *agent;
  InstanceProto *action;
} ActionProto__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "agent",
        .dataTypeSpecific.clazz = Nil,
        .number = ActionProto_FieldNumber_Agent,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(ActionProto__storage_, agent),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "action",
        .dataTypeSpecific.clazz = GPBObjCClass(InstanceProto),
        .number = ActionProto_FieldNumber_Action,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(ActionProto__storage_, action),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[ActionProto class]
                                     rootClass:[GymizeRoot class]
                                          file:GymizeRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(ActionProto__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - ObservationProto

@implementation ObservationProto

@dynamic hasLocator, locator;
@dynamic hasObservation, observation;

typedef struct ObservationProto__storage_ {
  uint32_t _has_storage_[1];
  LocatorProto *locator;
  InstanceProto *observation;
} ObservationProto__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "locator",
        .dataTypeSpecific.clazz = GPBObjCClass(LocatorProto),
        .number = ObservationProto_FieldNumber_Locator,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(ObservationProto__storage_, locator),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "observation",
        .dataTypeSpecific.clazz = GPBObjCClass(InstanceProto),
        .number = ObservationProto_FieldNumber_Observation,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(ObservationProto__storage_, observation),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[ObservationProto class]
                                     rootClass:[GymizeRoot class]
                                          file:GymizeRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(ObservationProto__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - RewardProto

@implementation RewardProto

@dynamic agent;
@dynamic reward;

typedef struct RewardProto__storage_ {
  uint32_t _has_storage_[1];
  NSString *agent;
  double reward;
} RewardProto__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "agent",
        .dataTypeSpecific.clazz = Nil,
        .number = RewardProto_FieldNumber_Agent,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(RewardProto__storage_, agent),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "reward",
        .dataTypeSpecific.clazz = Nil,
        .number = RewardProto_FieldNumber_Reward,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(RewardProto__storage_, reward),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeDouble,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[RewardProto class]
                                     rootClass:[GymizeRoot class]
                                          file:GymizeRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(RewardProto__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - InfoProto

@implementation InfoProto

@dynamic agent;
@dynamic infosArray, infosArray_Count;

typedef struct InfoProto__storage_ {
  uint32_t _has_storage_[1];
  NSString *agent;
  NSMutableArray *infosArray;
} InfoProto__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "agent",
        .dataTypeSpecific.clazz = Nil,
        .number = InfoProto_FieldNumber_Agent,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(InfoProto__storage_, agent),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "infosArray",
        .dataTypeSpecific.clazz = GPBObjCClass(InstanceProto),
        .number = InfoProto_FieldNumber_InfosArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(InfoProto__storage_, infosArray),
        .flags = GPBFieldRepeated,
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[InfoProto class]
                                     rootClass:[GymizeRoot class]
                                          file:GymizeRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(InfoProto__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - GymizeProto

@implementation GymizeProto

@dynamic resetAgentsArray, resetAgentsArray_Count;
@dynamic requestAgentsArray, requestAgentsArray_Count;
@dynamic actionsArray, actionsArray_Count;
@dynamic observationsArray, observationsArray_Count;
@dynamic rewardsArray, rewardsArray_Count;
@dynamic terminatedAgentsArray, terminatedAgentsArray_Count;
@dynamic truncatedAgentsArray, truncatedAgentsArray_Count;
@dynamic infosArray, infosArray_Count;
@dynamic hasRecording, recording;

typedef struct GymizeProto__storage_ {
  uint32_t _has_storage_[1];
  NSMutableArray *resetAgentsArray;
  NSMutableArray *requestAgentsArray;
  NSMutableArray *actionsArray;
  NSMutableArray *observationsArray;
  NSMutableArray *rewardsArray;
  NSMutableArray *terminatedAgentsArray;
  NSMutableArray *truncatedAgentsArray;
  NSMutableArray *infosArray;
  RecordingProto *recording;
} GymizeProto__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "resetAgentsArray",
        .dataTypeSpecific.clazz = Nil,
        .number = GymizeProto_FieldNumber_ResetAgentsArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(GymizeProto__storage_, resetAgentsArray),
        .flags = GPBFieldRepeated,
        .dataType = GPBDataTypeString,
      },
      {
        .name = "requestAgentsArray",
        .dataTypeSpecific.clazz = Nil,
        .number = GymizeProto_FieldNumber_RequestAgentsArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(GymizeProto__storage_, requestAgentsArray),
        .flags = GPBFieldRepeated,
        .dataType = GPBDataTypeString,
      },
      {
        .name = "actionsArray",
        .dataTypeSpecific.clazz = GPBObjCClass(ActionProto),
        .number = GymizeProto_FieldNumber_ActionsArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(GymizeProto__storage_, actionsArray),
        .flags = GPBFieldRepeated,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "observationsArray",
        .dataTypeSpecific.clazz = GPBObjCClass(ObservationProto),
        .number = GymizeProto_FieldNumber_ObservationsArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(GymizeProto__storage_, observationsArray),
        .flags = GPBFieldRepeated,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "rewardsArray",
        .dataTypeSpecific.clazz = GPBObjCClass(RewardProto),
        .number = GymizeProto_FieldNumber_RewardsArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(GymizeProto__storage_, rewardsArray),
        .flags = GPBFieldRepeated,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "terminatedAgentsArray",
        .dataTypeSpecific.clazz = Nil,
        .number = GymizeProto_FieldNumber_TerminatedAgentsArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(GymizeProto__storage_, terminatedAgentsArray),
        .flags = GPBFieldRepeated,
        .dataType = GPBDataTypeString,
      },
      {
        .name = "truncatedAgentsArray",
        .dataTypeSpecific.clazz = Nil,
        .number = GymizeProto_FieldNumber_TruncatedAgentsArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(GymizeProto__storage_, truncatedAgentsArray),
        .flags = GPBFieldRepeated,
        .dataType = GPBDataTypeString,
      },
      {
        .name = "infosArray",
        .dataTypeSpecific.clazz = GPBObjCClass(InfoProto),
        .number = GymizeProto_FieldNumber_InfosArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(GymizeProto__storage_, infosArray),
        .flags = GPBFieldRepeated,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "recording",
        .dataTypeSpecific.clazz = GPBObjCClass(RecordingProto),
        .number = GymizeProto_FieldNumber_Recording,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(GymizeProto__storage_, recording),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[GymizeProto class]
                                     rootClass:[GymizeRoot class]
                                          file:GymizeRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(GymizeProto__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end


#pragma clang diagnostic pop

// @@protoc_insertion_point(global_scope)