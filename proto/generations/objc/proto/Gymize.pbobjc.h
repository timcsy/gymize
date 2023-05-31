// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: gymize.proto

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

@class ActionProto;
@class InfoProto;
@class InstanceProto;
@class LocatorProto;
@class ObservationProto;
@class RecordingProto;
@class RewardProto;

NS_ASSUME_NONNULL_BEGIN

#pragma mark - GymizeRoot

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
GPB_FINAL @interface GymizeRoot : GPBRootObject
@end

#pragma mark - ActionProto

typedef GPB_ENUM(ActionProto_FieldNumber) {
  ActionProto_FieldNumber_Agent = 1,
  ActionProto_FieldNumber_Action = 2,
};

GPB_FINAL @interface ActionProto : GPBMessage

@property(nonatomic, readwrite, copy, null_resettable) NSString *agent;

@property(nonatomic, readwrite, strong, null_resettable) InstanceProto *action;
/** Test to see if @c action has been set. */
@property(nonatomic, readwrite) BOOL hasAction;

@end

#pragma mark - ObservationProto

typedef GPB_ENUM(ObservationProto_FieldNumber) {
  ObservationProto_FieldNumber_Locator = 1,
  ObservationProto_FieldNumber_Observation = 2,
};

GPB_FINAL @interface ObservationProto : GPBMessage

@property(nonatomic, readwrite, strong, null_resettable) LocatorProto *locator;
/** Test to see if @c locator has been set. */
@property(nonatomic, readwrite) BOOL hasLocator;

@property(nonatomic, readwrite, strong, null_resettable) InstanceProto *observation;
/** Test to see if @c observation has been set. */
@property(nonatomic, readwrite) BOOL hasObservation;

@end

#pragma mark - RewardProto

typedef GPB_ENUM(RewardProto_FieldNumber) {
  RewardProto_FieldNumber_Agent = 1,
  RewardProto_FieldNumber_Reward = 2,
};

GPB_FINAL @interface RewardProto : GPBMessage

@property(nonatomic, readwrite, copy, null_resettable) NSString *agent;

@property(nonatomic, readwrite) double reward;

@end

#pragma mark - InfoProto

typedef GPB_ENUM(InfoProto_FieldNumber) {
  InfoProto_FieldNumber_Agent = 1,
  InfoProto_FieldNumber_InfosArray = 2,
};

GPB_FINAL @interface InfoProto : GPBMessage

/** "" for env info */
@property(nonatomic, readwrite, copy, null_resettable) NSString *agent;

@property(nonatomic, readwrite, strong, null_resettable) NSMutableArray<InstanceProto*> *infosArray;
/** The number of items in @c infosArray without causing the array to be created. */
@property(nonatomic, readonly) NSUInteger infosArray_Count;

@end

#pragma mark - GymizeProto

typedef GPB_ENUM(GymizeProto_FieldNumber) {
  GymizeProto_FieldNumber_ResetAgentsArray = 1,
  GymizeProto_FieldNumber_RequestAgentsArray = 2,
  GymizeProto_FieldNumber_ActionsArray = 3,
  GymizeProto_FieldNumber_ObservationsArray = 4,
  GymizeProto_FieldNumber_RewardsArray = 5,
  GymizeProto_FieldNumber_TerminatedAgentsArray = 6,
  GymizeProto_FieldNumber_TruncatedAgentsArray = 7,
  GymizeProto_FieldNumber_InfosArray = 8,
  GymizeProto_FieldNumber_Recording = 9,
};

GPB_FINAL @interface GymizeProto : GPBMessage

/** "" for the environment, others for individual agents */
@property(nonatomic, readwrite, strong, null_resettable) NSMutableArray<NSString*> *resetAgentsArray;
/** The number of items in @c resetAgentsArray without causing the array to be created. */
@property(nonatomic, readonly) NSUInteger resetAgentsArray_Count;

/** ask for observations, rewards, infos or actions belong to agent name, "" will clear env info */
@property(nonatomic, readwrite, strong, null_resettable) NSMutableArray<NSString*> *requestAgentsArray;
/** The number of items in @c requestAgentsArray without causing the array to be created. */
@property(nonatomic, readonly) NSUInteger requestAgentsArray_Count;

@property(nonatomic, readwrite, strong, null_resettable) NSMutableArray<ActionProto*> *actionsArray;
/** The number of items in @c actionsArray without causing the array to be created. */
@property(nonatomic, readonly) NSUInteger actionsArray_Count;

@property(nonatomic, readwrite, strong, null_resettable) NSMutableArray<ObservationProto*> *observationsArray;
/** The number of items in @c observationsArray without causing the array to be created. */
@property(nonatomic, readonly) NSUInteger observationsArray_Count;

@property(nonatomic, readwrite, strong, null_resettable) NSMutableArray<RewardProto*> *rewardsArray;
/** The number of items in @c rewardsArray without causing the array to be created. */
@property(nonatomic, readonly) NSUInteger rewardsArray_Count;

/** "" for the environment, others for individual agents */
@property(nonatomic, readwrite, strong, null_resettable) NSMutableArray<NSString*> *terminatedAgentsArray;
/** The number of items in @c terminatedAgentsArray without causing the array to be created. */
@property(nonatomic, readonly) NSUInteger terminatedAgentsArray_Count;

/** "" for the environment, others for individual agents */
@property(nonatomic, readwrite, strong, null_resettable) NSMutableArray<NSString*> *truncatedAgentsArray;
/** The number of items in @c truncatedAgentsArray without causing the array to be created. */
@property(nonatomic, readonly) NSUInteger truncatedAgentsArray_Count;

/** can go with observations but do not need to go with actions */
@property(nonatomic, readwrite, strong, null_resettable) NSMutableArray<InfoProto*> *infosArray;
/** The number of items in @c infosArray without causing the array to be created. */
@property(nonatomic, readonly) NSUInteger infosArray_Count;

/** render images and audio */
@property(nonatomic, readwrite, strong, null_resettable) RecordingProto *recording;
/** Test to see if @c recording has been set. */
@property(nonatomic, readwrite) BOOL hasRecording;

@end

NS_ASSUME_NONNULL_END

CF_EXTERN_C_END

#pragma clang diagnostic pop

// @@protoc_insertion_point(global_scope)