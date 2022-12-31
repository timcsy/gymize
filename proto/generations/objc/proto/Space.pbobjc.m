// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: space.proto

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

#import <stdatomic.h>

#import "Space.pbobjc.h"
// @@protoc_insertion_point(imports)

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
#pragma clang diagnostic ignored "-Wdollar-in-identifier-extension"

#pragma mark - Objective C Class declarations
// Forward declarations of Objective C classes that we can use as
// static values in struct initializers.
// We don't use [Foo class] because it is not a static value.
GPBObjCClassDeclaration(Data);
GPBObjCClassDeclaration(Graph);
GPBObjCClassDeclaration(GraphSpace);
GPBObjCClassDeclaration(Image);
GPBObjCClassDeclaration(Space);
GPBObjCClassDeclaration(Tensor);

#pragma mark - SpaceRoot

@implementation SpaceRoot

// No extensions in the file and no imports, so no need to generate
// +extensionRegistry.

@end

#pragma mark - SpaceRoot_FileDescriptor

static GPBFileDescriptor *SpaceRoot_FileDescriptor(void) {
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

#pragma mark - Enum DataType

GPBEnumDescriptor *DataType_EnumDescriptor(void) {
  static _Atomic(GPBEnumDescriptor*) descriptor = nil;
  if (!descriptor) {
    static const char *valueNames =
        "DataTypeUnspecified\000DataTypeFloat\000DataTy"
        "peDouble\000DataTypeInt\000DataTypeLong\000DataTy"
        "peUint\000DataTypeUlong\000DataTypeBool\000";
    static const int32_t values[] = {
        DataType_DataTypeUnspecified,
        DataType_DataTypeFloat,
        DataType_DataTypeDouble,
        DataType_DataTypeInt,
        DataType_DataTypeLong,
        DataType_DataTypeUint,
        DataType_DataTypeUlong,
        DataType_DataTypeBool,
    };
    GPBEnumDescriptor *worker =
        [GPBEnumDescriptor allocDescriptorForName:GPBNSStringifySymbol(DataType)
                                       valueNames:valueNames
                                           values:values
                                            count:(uint32_t)(sizeof(values) / sizeof(int32_t))
                                     enumVerifier:DataType_IsValidValue];
    GPBEnumDescriptor *expected = nil;
    if (!atomic_compare_exchange_strong(&descriptor, &expected, worker)) {
      [worker release];
    }
  }
  return descriptor;
}

BOOL DataType_IsValidValue(int32_t value__) {
  switch (value__) {
    case DataType_DataTypeUnspecified:
    case DataType_DataTypeFloat:
    case DataType_DataTypeDouble:
    case DataType_DataTypeInt:
    case DataType_DataTypeLong:
    case DataType_DataTypeUint:
    case DataType_DataTypeUlong:
    case DataType_DataTypeBool:
      return YES;
    default:
      return NO;
  }
}

#pragma mark - Enum CompressionType

GPBEnumDescriptor *CompressionType_EnumDescriptor(void) {
  static _Atomic(GPBEnumDescriptor*) descriptor = nil;
  if (!descriptor) {
    static const char *valueNames =
        "CompressionTypeUnspecified\000CompressionTy"
        "peNone\000CompressionTypePng\000CompressionTyp"
        "eJpg\000";
    static const int32_t values[] = {
        CompressionType_CompressionTypeUnspecified,
        CompressionType_CompressionTypeNone,
        CompressionType_CompressionTypePng,
        CompressionType_CompressionTypeJpg,
    };
    GPBEnumDescriptor *worker =
        [GPBEnumDescriptor allocDescriptorForName:GPBNSStringifySymbol(CompressionType)
                                       valueNames:valueNames
                                           values:values
                                            count:(uint32_t)(sizeof(values) / sizeof(int32_t))
                                     enumVerifier:CompressionType_IsValidValue];
    GPBEnumDescriptor *expected = nil;
    if (!atomic_compare_exchange_strong(&descriptor, &expected, worker)) {
      [worker release];
    }
  }
  return descriptor;
}

BOOL CompressionType_IsValidValue(int32_t value__) {
  switch (value__) {
    case CompressionType_CompressionTypeUnspecified:
    case CompressionType_CompressionTypeNone:
    case CompressionType_CompressionTypePng:
    case CompressionType_CompressionTypeJpg:
      return YES;
    default:
      return NO;
  }
}

#pragma mark - Enum SpaceType

GPBEnumDescriptor *SpaceType_EnumDescriptor(void) {
  static _Atomic(GPBEnumDescriptor*) descriptor = nil;
  if (!descriptor) {
    static const char *valueNames =
        "SpaceTypeUnspecified\000SpaceTypeRaw\000SpaceT"
        "ypeBox\000SpaceTypeDiscrete\000SpaceTypeMultiB"
        "inary\000SpaceTypeMultiDiscrete\000SpaceTypeTe"
        "xt\000SpaceTypeDict\000SpaceTypeTuple\000SpaceTyp"
        "eSequence\000SpaceTypeGraph\000SpaceTypeImage\000";
    static const int32_t values[] = {
        SpaceType_SpaceTypeUnspecified,
        SpaceType_SpaceTypeRaw,
        SpaceType_SpaceTypeBox,
        SpaceType_SpaceTypeDiscrete,
        SpaceType_SpaceTypeMultiBinary,
        SpaceType_SpaceTypeMultiDiscrete,
        SpaceType_SpaceTypeText,
        SpaceType_SpaceTypeDict,
        SpaceType_SpaceTypeTuple,
        SpaceType_SpaceTypeSequence,
        SpaceType_SpaceTypeGraph,
        SpaceType_SpaceTypeImage,
    };
    GPBEnumDescriptor *worker =
        [GPBEnumDescriptor allocDescriptorForName:GPBNSStringifySymbol(SpaceType)
                                       valueNames:valueNames
                                           values:values
                                            count:(uint32_t)(sizeof(values) / sizeof(int32_t))
                                     enumVerifier:SpaceType_IsValidValue];
    GPBEnumDescriptor *expected = nil;
    if (!atomic_compare_exchange_strong(&descriptor, &expected, worker)) {
      [worker release];
    }
  }
  return descriptor;
}

BOOL SpaceType_IsValidValue(int32_t value__) {
  switch (value__) {
    case SpaceType_SpaceTypeUnspecified:
    case SpaceType_SpaceTypeRaw:
    case SpaceType_SpaceTypeBox:
    case SpaceType_SpaceTypeDiscrete:
    case SpaceType_SpaceTypeMultiBinary:
    case SpaceType_SpaceTypeMultiDiscrete:
    case SpaceType_SpaceTypeText:
    case SpaceType_SpaceTypeDict:
    case SpaceType_SpaceTypeTuple:
    case SpaceType_SpaceTypeSequence:
    case SpaceType_SpaceTypeGraph:
    case SpaceType_SpaceTypeImage:
      return YES;
    default:
      return NO;
  }
}

#pragma mark - Tensor

@implementation Tensor

@dynamic shapeArray, shapeArray_Count;
@dynamic dataType;
@dynamic floatArrayArray, floatArrayArray_Count;
@dynamic doubleArrayArray, doubleArrayArray_Count;
@dynamic intArrayArray, intArrayArray_Count;
@dynamic longArrayArray, longArrayArray_Count;
@dynamic unsignedIntArrayArray, unsignedIntArrayArray_Count;
@dynamic unsignedLongArrayArray, unsignedLongArrayArray_Count;
@dynamic boolArrayArray, boolArrayArray_Count;

typedef struct Tensor__storage_ {
  uint32_t _has_storage_[1];
  DataType dataType;
  GPBInt32Array *shapeArray;
  GPBFloatArray *floatArrayArray;
  GPBDoubleArray *doubleArrayArray;
  GPBInt32Array *intArrayArray;
  GPBInt64Array *longArrayArray;
  GPBUInt32Array *unsignedIntArrayArray;
  GPBUInt64Array *unsignedLongArrayArray;
  GPBBoolArray *boolArrayArray;
} Tensor__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "shapeArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Tensor_FieldNumber_ShapeArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Tensor__storage_, shapeArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "dataType",
        .dataTypeSpecific.enumDescFunc = DataType_EnumDescriptor,
        .number = Tensor_FieldNumber_DataType,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(Tensor__storage_, dataType),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldHasEnumDescriptor | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeEnum,
      },
      {
        .name = "floatArrayArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Tensor_FieldNumber_FloatArrayArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Tensor__storage_, floatArrayArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeFloat,
      },
      {
        .name = "doubleArrayArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Tensor_FieldNumber_DoubleArrayArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Tensor__storage_, doubleArrayArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeDouble,
      },
      {
        .name = "intArrayArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Tensor_FieldNumber_IntArrayArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Tensor__storage_, intArrayArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeSInt32,
      },
      {
        .name = "longArrayArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Tensor_FieldNumber_LongArrayArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Tensor__storage_, longArrayArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeSInt64,
      },
      {
        .name = "unsignedIntArrayArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Tensor_FieldNumber_UnsignedIntArrayArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Tensor__storage_, unsignedIntArrayArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeUInt32,
      },
      {
        .name = "unsignedLongArrayArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Tensor_FieldNumber_UnsignedLongArrayArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Tensor__storage_, unsignedLongArrayArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeUInt64,
      },
      {
        .name = "boolArrayArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Tensor_FieldNumber_BoolArrayArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Tensor__storage_, boolArrayArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeBool,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[Tensor class]
                                     rootClass:[SpaceRoot class]
                                          file:SpaceRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(Tensor__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

int32_t Tensor_DataType_RawValue(Tensor *message) {
  GPBDescriptor *descriptor = [Tensor descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:Tensor_FieldNumber_DataType];
  return GPBGetMessageRawEnumField(message, field);
}

void SetTensor_DataType_RawValue(Tensor *message, int32_t value) {
  GPBDescriptor *descriptor = [Tensor descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:Tensor_FieldNumber_DataType];
  GPBSetMessageRawEnumField(message, field, value);
}

#pragma mark - GraphSpace

@implementation GraphSpace

@dynamic hasNodeSpace, nodeSpace;
@dynamic hasEdgeSpace, edgeSpace;

typedef struct GraphSpace__storage_ {
  uint32_t _has_storage_[1];
  Space *nodeSpace;
  Space *edgeSpace;
} GraphSpace__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "nodeSpace",
        .dataTypeSpecific.clazz = GPBObjCClass(Space),
        .number = GraphSpace_FieldNumber_NodeSpace,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(GraphSpace__storage_, nodeSpace),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "edgeSpace",
        .dataTypeSpecific.clazz = GPBObjCClass(Space),
        .number = GraphSpace_FieldNumber_EdgeSpace,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(GraphSpace__storage_, edgeSpace),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[GraphSpace class]
                                     rootClass:[SpaceRoot class]
                                          file:SpaceRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(GraphSpace__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - Graph

@implementation Graph

@dynamic hasNodes, nodes;
@dynamic hasEdges, edges;
@dynamic hasEdgeLinks, edgeLinks;

typedef struct Graph__storage_ {
  uint32_t _has_storage_[1];
  Tensor *nodes;
  Tensor *edges;
  Tensor *edgeLinks;
} Graph__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "nodes",
        .dataTypeSpecific.clazz = GPBObjCClass(Tensor),
        .number = Graph_FieldNumber_Nodes,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(Graph__storage_, nodes),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "edges",
        .dataTypeSpecific.clazz = GPBObjCClass(Tensor),
        .number = Graph_FieldNumber_Edges,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(Graph__storage_, edges),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "edgeLinks",
        .dataTypeSpecific.clazz = GPBObjCClass(Tensor),
        .number = Graph_FieldNumber_EdgeLinks,
        .hasIndex = 2,
        .offset = (uint32_t)offsetof(Graph__storage_, edgeLinks),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[Graph class]
                                     rootClass:[SpaceRoot class]
                                          file:SpaceRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(Graph__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - Image

@implementation Image

@dynamic compressionType;
@dynamic data_p;
@dynamic shapeArray, shapeArray_Count;
@dynamic dimensionMappingArray, dimensionMappingArray_Count;

typedef struct Image__storage_ {
  uint32_t _has_storage_[1];
  CompressionType compressionType;
  NSData *data_p;
  GPBInt32Array *shapeArray;
  GPBInt32Array *dimensionMappingArray;
} Image__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "compressionType",
        .dataTypeSpecific.enumDescFunc = CompressionType_EnumDescriptor,
        .number = Image_FieldNumber_CompressionType,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(Image__storage_, compressionType),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldHasEnumDescriptor | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeEnum,
      },
      {
        .name = "data_p",
        .dataTypeSpecific.clazz = Nil,
        .number = Image_FieldNumber_Data_p,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(Image__storage_, data_p),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeBytes,
      },
      {
        .name = "shapeArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Image_FieldNumber_ShapeArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Image__storage_, shapeArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "dimensionMappingArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Image_FieldNumber_DimensionMappingArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Image__storage_, dimensionMappingArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeInt32,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[Image class]
                                     rootClass:[SpaceRoot class]
                                          file:SpaceRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(Image__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

int32_t Image_CompressionType_RawValue(Image *message) {
  GPBDescriptor *descriptor = [Image descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:Image_FieldNumber_CompressionType];
  return GPBGetMessageRawEnumField(message, field);
}

void SetImage_CompressionType_RawValue(Image *message, int32_t value) {
  GPBDescriptor *descriptor = [Image descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:Image_FieldNumber_CompressionType];
  GPBSetMessageRawEnumField(message, field, value);
}

#pragma mark - Space

@implementation Space

@dynamic spaceType;
@dynamic description_p;
@dynamic shapeArray, shapeArray_Count;
@dynamic dataType;
@dynamic lowArray, lowArray_Count;
@dynamic highArray, highArray_Count;
@dynamic min;
@dynamic max;
@dynamic nvecArray, nvecArray_Count;
@dynamic charsetArray, charsetArray_Count;
@dynamic dictSpace, dictSpace_Count;
@dynamic listSpace, listSpace_Count;
@dynamic hasGraphSpace, graphSpace;

typedef struct Space__storage_ {
  uint32_t _has_storage_[1];
  SpaceType spaceType;
  int32_t min;
  int32_t max;
  NSString *description_p;
  GPBInt32Array *shapeArray;
  NSString *dataType;
  GPBFloatArray *lowArray;
  GPBFloatArray *highArray;
  GPBInt32Array *nvecArray;
  NSMutableArray *charsetArray;
  NSMutableDictionary *dictSpace;
  GPBInt32ObjectDictionary *listSpace;
  GraphSpace *graphSpace;
} Space__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "spaceType",
        .dataTypeSpecific.enumDescFunc = SpaceType_EnumDescriptor,
        .number = Space_FieldNumber_SpaceType,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(Space__storage_, spaceType),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldHasEnumDescriptor | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeEnum,
      },
      {
        .name = "description_p",
        .dataTypeSpecific.clazz = Nil,
        .number = Space_FieldNumber_Description_p,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(Space__storage_, description_p),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "shapeArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Space_FieldNumber_ShapeArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Space__storage_, shapeArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "dataType",
        .dataTypeSpecific.clazz = Nil,
        .number = Space_FieldNumber_DataType,
        .hasIndex = 2,
        .offset = (uint32_t)offsetof(Space__storage_, dataType),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "lowArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Space_FieldNumber_LowArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Space__storage_, lowArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeFloat,
      },
      {
        .name = "highArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Space_FieldNumber_HighArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Space__storage_, highArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeFloat,
      },
      {
        .name = "min",
        .dataTypeSpecific.clazz = Nil,
        .number = Space_FieldNumber_Min,
        .hasIndex = 3,
        .offset = (uint32_t)offsetof(Space__storage_, min),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "max",
        .dataTypeSpecific.clazz = Nil,
        .number = Space_FieldNumber_Max,
        .hasIndex = 4,
        .offset = (uint32_t)offsetof(Space__storage_, max),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "nvecArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Space_FieldNumber_NvecArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Space__storage_, nvecArray),
        .flags = (GPBFieldFlags)(GPBFieldRepeated | GPBFieldPacked),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "charsetArray",
        .dataTypeSpecific.clazz = Nil,
        .number = Space_FieldNumber_CharsetArray,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Space__storage_, charsetArray),
        .flags = GPBFieldRepeated,
        .dataType = GPBDataTypeString,
      },
      {
        .name = "dictSpace",
        .dataTypeSpecific.clazz = GPBObjCClass(Space),
        .number = Space_FieldNumber_DictSpace,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Space__storage_, dictSpace),
        .flags = GPBFieldMapKeyString,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "listSpace",
        .dataTypeSpecific.clazz = GPBObjCClass(Space),
        .number = Space_FieldNumber_ListSpace,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Space__storage_, listSpace),
        .flags = GPBFieldMapKeyInt32,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "graphSpace",
        .dataTypeSpecific.clazz = GPBObjCClass(GraphSpace),
        .number = Space_FieldNumber_GraphSpace,
        .hasIndex = 5,
        .offset = (uint32_t)offsetof(Space__storage_, graphSpace),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[Space class]
                                     rootClass:[SpaceRoot class]
                                          file:SpaceRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(Space__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

int32_t Space_SpaceType_RawValue(Space *message) {
  GPBDescriptor *descriptor = [Space descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:Space_FieldNumber_SpaceType];
  return GPBGetMessageRawEnumField(message, field);
}

void SetSpace_SpaceType_RawValue(Space *message, int32_t value) {
  GPBDescriptor *descriptor = [Space descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:Space_FieldNumber_SpaceType];
  GPBSetMessageRawEnumField(message, field, value);
}

#pragma mark - Data

@implementation Data

@dynamic spaceType;
@dynamic dataType;
@dynamic rawData;
@dynamic hasBox, box;
@dynamic discrete;
@dynamic hasMultiBinary, multiBinary;
@dynamic hasMultiDiscrete, multiDiscrete;
@dynamic text;
@dynamic dict, dict_Count;
@dynamic list, list_Count;
@dynamic hasGraph, graph;
@dynamic hasImage, image;

typedef struct Data__storage_ {
  uint32_t _has_storage_[1];
  SpaceType spaceType;
  int32_t discrete;
  NSString *dataType;
  NSData *rawData;
  Tensor *box;
  Tensor *multiBinary;
  Tensor *multiDiscrete;
  NSString *text;
  NSMutableDictionary *dict;
  GPBInt32ObjectDictionary *list;
  Graph *graph;
  Image *image;
} Data__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "spaceType",
        .dataTypeSpecific.enumDescFunc = SpaceType_EnumDescriptor,
        .number = Data_FieldNumber_SpaceType,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(Data__storage_, spaceType),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldHasEnumDescriptor | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeEnum,
      },
      {
        .name = "dataType",
        .dataTypeSpecific.clazz = Nil,
        .number = Data_FieldNumber_DataType,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(Data__storage_, dataType),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "rawData",
        .dataTypeSpecific.clazz = Nil,
        .number = Data_FieldNumber_RawData,
        .hasIndex = 2,
        .offset = (uint32_t)offsetof(Data__storage_, rawData),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeBytes,
      },
      {
        .name = "box",
        .dataTypeSpecific.clazz = GPBObjCClass(Tensor),
        .number = Data_FieldNumber_Box,
        .hasIndex = 3,
        .offset = (uint32_t)offsetof(Data__storage_, box),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "discrete",
        .dataTypeSpecific.clazz = Nil,
        .number = Data_FieldNumber_Discrete,
        .hasIndex = 4,
        .offset = (uint32_t)offsetof(Data__storage_, discrete),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "multiBinary",
        .dataTypeSpecific.clazz = GPBObjCClass(Tensor),
        .number = Data_FieldNumber_MultiBinary,
        .hasIndex = 5,
        .offset = (uint32_t)offsetof(Data__storage_, multiBinary),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "multiDiscrete",
        .dataTypeSpecific.clazz = GPBObjCClass(Tensor),
        .number = Data_FieldNumber_MultiDiscrete,
        .hasIndex = 6,
        .offset = (uint32_t)offsetof(Data__storage_, multiDiscrete),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "text",
        .dataTypeSpecific.clazz = Nil,
        .number = Data_FieldNumber_Text,
        .hasIndex = 7,
        .offset = (uint32_t)offsetof(Data__storage_, text),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "dict",
        .dataTypeSpecific.clazz = GPBObjCClass(Data),
        .number = Data_FieldNumber_Dict,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Data__storage_, dict),
        .flags = GPBFieldMapKeyString,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "list",
        .dataTypeSpecific.clazz = GPBObjCClass(Data),
        .number = Data_FieldNumber_List,
        .hasIndex = GPBNoHasBit,
        .offset = (uint32_t)offsetof(Data__storage_, list),
        .flags = GPBFieldMapKeyInt32,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "graph",
        .dataTypeSpecific.clazz = GPBObjCClass(Graph),
        .number = Data_FieldNumber_Graph,
        .hasIndex = 8,
        .offset = (uint32_t)offsetof(Data__storage_, graph),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "image",
        .dataTypeSpecific.clazz = GPBObjCClass(Image),
        .number = Data_FieldNumber_Image,
        .hasIndex = 9,
        .offset = (uint32_t)offsetof(Data__storage_, image),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[Data class]
                                     rootClass:[SpaceRoot class]
                                          file:SpaceRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(Data__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

int32_t Data_SpaceType_RawValue(Data *message) {
  GPBDescriptor *descriptor = [Data descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:Data_FieldNumber_SpaceType];
  return GPBGetMessageRawEnumField(message, field);
}

void SetData_SpaceType_RawValue(Data *message, int32_t value) {
  GPBDescriptor *descriptor = [Data descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:Data_FieldNumber_SpaceType];
  GPBSetMessageRawEnumField(message, field, value);
}


#pragma clang diagnostic pop

// @@protoc_insertion_point(global_scope)
