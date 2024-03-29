// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: signaling.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Gymize.Protobuf {

  /// <summary>Holder for reflection information generated from signaling.proto</summary>
  public static partial class SignalingReflection {

    #region Descriptor
    /// <summary>File descriptor for signaling.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static SignalingReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9zaWduYWxpbmcucHJvdG8ijAEKC1NpZ25hbFByb3RvEiUKC3NpZ25hbF90",
            "eXBlGAEgASgOMhAuU2lnbmFsVHlwZVByb3RvEgoKAmlkGAIgASgJEgwKBGRh",
            "dGEYAyABKAwSDAoEbmFtZRgEIAEoCRIhCglwZWVyX3R5cGUYBSABKA4yDi5Q",
            "ZWVyVHlwZVByb3RvEgsKA3VybBgGIAEoCSqpAQoPU2lnbmFsVHlwZVByb3Rv",
            "EiEKHVNJR05BTF9UWVBFX1BST1RPX1VOU1BFQ0lGSUVEEAASGgoWU0lHTkFM",
            "X1RZUEVfUFJPVE9fSU5JVBABEhwKGFNJR05BTF9UWVBFX1BST1RPX1VQREFU",
            "RRACEhwKGFNJR05BTF9UWVBFX1BST1RPX1JFU1VNRRADEhsKF1NJR05BTF9U",
            "WVBFX1BST1RPX0NMT1NFEAQqaQoNUGVlclR5cGVQcm90bxIfChtQRUVSX1RZ",
            "UEVfUFJPVE9fVU5TUEVDSUZJRUQQABIaChZQRUVSX1RZUEVfUFJPVE9fQUNU",
            "SVZFEAESGwoXUEVFUl9UWVBFX1BST1RPX1BBU1NJVkUQAkISqgIPR3ltaXpl",
            "LlByb3RvYnVmYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Gymize.Protobuf.SignalTypeProto), typeof(global::Gymize.Protobuf.PeerTypeProto), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Gymize.Protobuf.SignalProto), global::Gymize.Protobuf.SignalProto.Parser, new[]{ "SignalType", "Id", "Data", "Name", "PeerType", "Url" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum SignalTypeProto {
    [pbr::OriginalName("SIGNAL_TYPE_PROTO_UNSPECIFIED")] Unspecified = 0,
    [pbr::OriginalName("SIGNAL_TYPE_PROTO_INIT")] Init = 1,
    /// <summary>
    /// update the peer server information
    /// </summary>
    [pbr::OriginalName("SIGNAL_TYPE_PROTO_UPDATE")] Update = 2,
    /// <summary>
    /// resume the connection with same signal id
    /// </summary>
    [pbr::OriginalName("SIGNAL_TYPE_PROTO_RESUME")] Resume = 3,
    /// <summary>
    /// ask another peer to close, and remove signal id
    /// </summary>
    [pbr::OriginalName("SIGNAL_TYPE_PROTO_CLOSE")] Close = 4,
  }

  public enum PeerTypeProto {
    [pbr::OriginalName("PEER_TYPE_PROTO_UNSPECIFIED")] Unspecified = 0,
    [pbr::OriginalName("PEER_TYPE_PROTO_ACTIVE")] Active = 1,
    [pbr::OriginalName("PEER_TYPE_PROTO_PASSIVE")] Passive = 2,
  }

  #endregion

  #region Messages
  public sealed partial class SignalProto : pb::IMessage<SignalProto>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<SignalProto> _parser = new pb::MessageParser<SignalProto>(() => new SignalProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<SignalProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Gymize.Protobuf.SignalingReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SignalProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SignalProto(SignalProto other) : this() {
      signalType_ = other.signalType_;
      id_ = other.id_;
      data_ = other.data_;
      name_ = other.name_;
      peerType_ = other.peerType_;
      url_ = other.url_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SignalProto Clone() {
      return new SignalProto(this);
    }

    /// <summary>Field number for the "signal_type" field.</summary>
    public const int SignalTypeFieldNumber = 1;
    private global::Gymize.Protobuf.SignalTypeProto signalType_ = global::Gymize.Protobuf.SignalTypeProto.Unspecified;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Gymize.Protobuf.SignalTypeProto SignalType {
      get { return signalType_; }
      set {
        signalType_ = value;
      }
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 2;
    private string id_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Id {
      get { return id_; }
      set {
        id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "data" field.</summary>
    public const int DataFieldNumber = 3;
    private pb::ByteString data_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString Data {
      get { return data_; }
      set {
        data_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 4;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "peer_type" field.</summary>
    public const int PeerTypeFieldNumber = 5;
    private global::Gymize.Protobuf.PeerTypeProto peerType_ = global::Gymize.Protobuf.PeerTypeProto.Unspecified;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Gymize.Protobuf.PeerTypeProto PeerType {
      get { return peerType_; }
      set {
        peerType_ = value;
      }
    }

    /// <summary>Field number for the "url" field.</summary>
    public const int UrlFieldNumber = 6;
    private string url_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Url {
      get { return url_; }
      set {
        url_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as SignalProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(SignalProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (SignalType != other.SignalType) return false;
      if (Id != other.Id) return false;
      if (Data != other.Data) return false;
      if (Name != other.Name) return false;
      if (PeerType != other.PeerType) return false;
      if (Url != other.Url) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (SignalType != global::Gymize.Protobuf.SignalTypeProto.Unspecified) hash ^= SignalType.GetHashCode();
      if (Id.Length != 0) hash ^= Id.GetHashCode();
      if (Data.Length != 0) hash ^= Data.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (PeerType != global::Gymize.Protobuf.PeerTypeProto.Unspecified) hash ^= PeerType.GetHashCode();
      if (Url.Length != 0) hash ^= Url.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (SignalType != global::Gymize.Protobuf.SignalTypeProto.Unspecified) {
        output.WriteRawTag(8);
        output.WriteEnum((int) SignalType);
      }
      if (Id.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Id);
      }
      if (Data.Length != 0) {
        output.WriteRawTag(26);
        output.WriteBytes(Data);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Name);
      }
      if (PeerType != global::Gymize.Protobuf.PeerTypeProto.Unspecified) {
        output.WriteRawTag(40);
        output.WriteEnum((int) PeerType);
      }
      if (Url.Length != 0) {
        output.WriteRawTag(50);
        output.WriteString(Url);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (SignalType != global::Gymize.Protobuf.SignalTypeProto.Unspecified) {
        output.WriteRawTag(8);
        output.WriteEnum((int) SignalType);
      }
      if (Id.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Id);
      }
      if (Data.Length != 0) {
        output.WriteRawTag(26);
        output.WriteBytes(Data);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Name);
      }
      if (PeerType != global::Gymize.Protobuf.PeerTypeProto.Unspecified) {
        output.WriteRawTag(40);
        output.WriteEnum((int) PeerType);
      }
      if (Url.Length != 0) {
        output.WriteRawTag(50);
        output.WriteString(Url);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (SignalType != global::Gymize.Protobuf.SignalTypeProto.Unspecified) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) SignalType);
      }
      if (Id.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Id);
      }
      if (Data.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Data);
      }
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (PeerType != global::Gymize.Protobuf.PeerTypeProto.Unspecified) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) PeerType);
      }
      if (Url.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Url);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(SignalProto other) {
      if (other == null) {
        return;
      }
      if (other.SignalType != global::Gymize.Protobuf.SignalTypeProto.Unspecified) {
        SignalType = other.SignalType;
      }
      if (other.Id.Length != 0) {
        Id = other.Id;
      }
      if (other.Data.Length != 0) {
        Data = other.Data;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.PeerType != global::Gymize.Protobuf.PeerTypeProto.Unspecified) {
        PeerType = other.PeerType;
      }
      if (other.Url.Length != 0) {
        Url = other.Url;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            SignalType = (global::Gymize.Protobuf.SignalTypeProto) input.ReadEnum();
            break;
          }
          case 18: {
            Id = input.ReadString();
            break;
          }
          case 26: {
            Data = input.ReadBytes();
            break;
          }
          case 34: {
            Name = input.ReadString();
            break;
          }
          case 40: {
            PeerType = (global::Gymize.Protobuf.PeerTypeProto) input.ReadEnum();
            break;
          }
          case 50: {
            Url = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            SignalType = (global::Gymize.Protobuf.SignalTypeProto) input.ReadEnum();
            break;
          }
          case 18: {
            Id = input.ReadString();
            break;
          }
          case 26: {
            Data = input.ReadBytes();
            break;
          }
          case 34: {
            Name = input.ReadString();
            break;
          }
          case 40: {
            PeerType = (global::Gymize.Protobuf.PeerTypeProto) input.ReadEnum();
            break;
          }
          case 50: {
            Url = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
