# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: signaling.proto

require 'google/protobuf'

Google::Protobuf::DescriptorPool.generated_pool.build do
  add_file("signaling.proto", :syntax => :proto3) do
    add_message "Signal" do
      optional :signal_type, :enum, 1, "SignalType"
      optional :id, :string, 2
      optional :data, :bytes, 3
      optional :name, :string, 4
      optional :peer_type, :enum, 5, "PeerType"
      optional :url, :string, 6
    end
    add_enum "SignalType" do
      value :SIGNAL_TYPE_UNSPECIFIED, 0
      value :SIGNAL_TYPE_INIT, 1
      value :SIGNAL_TYPE_UPDATE, 2
      value :SIGNAL_TYPE_RESUME, 3
      value :SIGNAL_TYPE_CLOSE, 4
    end
    add_enum "PeerType" do
      value :PEER_TYPE_UNSPECIFIED, 0
      value :PEER_TYPE_ACTIVE, 1
      value :PEER_TYPE_PASSIVE, 2
    end
  end
end

Signal = ::Google::Protobuf::DescriptorPool.generated_pool.lookup("Signal").msgclass
SignalType = ::Google::Protobuf::DescriptorPool.generated_pool.lookup("SignalType").enummodule
PeerType = ::Google::Protobuf::DescriptorPool.generated_pool.lookup("PeerType").enummodule
