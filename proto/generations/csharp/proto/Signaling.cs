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
namespace PAIA.Gymize.Signaling {

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
            "Cg9zaWduYWxpbmcucHJvdG8ifQoGU2lnbmFsEiAKC3NpZ25hbF90eXBlGAEg",
            "ASgOMgsuU2lnbmFsVHlwZRIKCgJpZBgCIAEoCRIMCgRkYXRhGAMgASgMEgwK",
            "BG5hbWUYBCABKAkSHAoJcGVlcl90eXBlGAUgASgOMgkuUGVlclR5cGUSCwoD",
            "dXJsGAYgASgJKoYBCgpTaWduYWxUeXBlEhsKF1NJR05BTF9UWVBFX1VOU1BF",
            "Q0lGSUVEEAASFAoQU0lHTkFMX1RZUEVfSU5JVBABEhYKElNJR05BTF9UWVBF",
            "X1VQREFURRACEhYKElNJR05BTF9UWVBFX1JFU1VNRRADEhUKEVNJR05BTF9U",
            "WVBFX0NMT1NFEAQqUgoIUGVlclR5cGUSGQoVUEVFUl9UWVBFX1VOU1BFQ0lG",
            "SUVEEAASFAoQUEVFUl9UWVBFX0FDVElWRRABEhUKEVBFRVJfVFlQRV9QQVNT",
            "SVZFEAJCGKoCFVBBSUEuR3ltaXplLlNpZ25hbGluZ2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::PAIA.Gymize.Signaling.SignalType), typeof(global::PAIA.Gymize.Signaling.PeerType), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::PAIA.Gymize.Signaling.Signal), global::PAIA.Gymize.Signaling.Signal.Parser, new[]{ "SignalType", "Id", "Data", "Name", "PeerType", "Url" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum SignalType {
    [pbr::OriginalName("SIGNAL_TYPE_UNSPECIFIED")] Unspecified = 0,
    [pbr::OriginalName("SIGNAL_TYPE_INIT")] Init = 1,
    /// <summary>
    /// update the peer server information
    /// </summary>
    [pbr::OriginalName("SIGNAL_TYPE_UPDATE")] Update = 2,
    /// <summary>
    /// resume the connection with same signal id
    /// </summary>
    [pbr::OriginalName("SIGNAL_TYPE_RESUME")] Resume = 3,
    /// <summary>
    /// ask another peer to close, and remove signal id
    /// </summary>
    [pbr::OriginalName("SIGNAL_TYPE_CLOSE")] Close = 4,
  }

  public enum PeerType {
    [pbr::OriginalName("PEER_TYPE_UNSPECIFIED")] Unspecified = 0,
    [pbr::OriginalName("PEER_TYPE_ACTIVE")] Active = 1,
    [pbr::OriginalName("PEER_TYPE_PASSIVE")] Passive = 2,
  }

  #endregion

  #region Messages
  public sealed partial class Signal : pb::IMessage<Signal>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<Signal> _parser = new pb::MessageParser<Signal>(() => new Signal());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<Signal> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::PAIA.Gymize.Signaling.SignalingReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Signal() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Signal(Signal other) : this() {
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
    public Signal Clone() {
      return new Signal(this);
    }

    /// <summary>Field number for the "signal_type" field.</summary>
    public const int SignalTypeFieldNumber = 1;
    private global::PAIA.Gymize.Signaling.SignalType signalType_ = global::PAIA.Gymize.Signaling.SignalType.Unspecified;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::PAIA.Gymize.Signaling.SignalType SignalType {
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
    private global::PAIA.Gymize.Signaling.PeerType peerType_ = global::PAIA.Gymize.Signaling.PeerType.Unspecified;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::PAIA.Gymize.Signaling.PeerType PeerType {
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
      return Equals(other as Signal);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(Signal other) {
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
      if (SignalType != global::PAIA.Gymize.Signaling.SignalType.Unspecified) hash ^= SignalType.GetHashCode();
      if (Id.Length != 0) hash ^= Id.GetHashCode();
      if (Data.Length != 0) hash ^= Data.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (PeerType != global::PAIA.Gymize.Signaling.PeerType.Unspecified) hash ^= PeerType.GetHashCode();
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
      if (SignalType != global::PAIA.Gymize.Signaling.SignalType.Unspecified) {
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
      if (PeerType != global::PAIA.Gymize.Signaling.PeerType.Unspecified) {
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
      if (SignalType != global::PAIA.Gymize.Signaling.SignalType.Unspecified) {
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
      if (PeerType != global::PAIA.Gymize.Signaling.PeerType.Unspecified) {
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
      if (SignalType != global::PAIA.Gymize.Signaling.SignalType.Unspecified) {
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
      if (PeerType != global::PAIA.Gymize.Signaling.PeerType.Unspecified) {
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
    public void MergeFrom(Signal other) {
      if (other == null) {
        return;
      }
      if (other.SignalType != global::PAIA.Gymize.Signaling.SignalType.Unspecified) {
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
      if (other.PeerType != global::PAIA.Gymize.Signaling.PeerType.Unspecified) {
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
            SignalType = (global::PAIA.Gymize.Signaling.SignalType) input.ReadEnum();
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
            PeerType = (global::PAIA.Gymize.Signaling.PeerType) input.ReadEnum();
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
            SignalType = (global::PAIA.Gymize.Signaling.SignalType) input.ReadEnum();
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
            PeerType = (global::PAIA.Gymize.Signaling.PeerType) input.ReadEnum();
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
