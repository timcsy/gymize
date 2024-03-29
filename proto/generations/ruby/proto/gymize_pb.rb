# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: gymize.proto

require 'google/protobuf'

require 'space_pb'
require 'locator_pb'
require 'render_pb'

Google::Protobuf::DescriptorPool.generated_pool.build do
  add_file("gymize.proto", :syntax => :proto3) do
    add_message "ActionProto" do
      optional :agent, :string, 1
      optional :action, :message, 2, "InstanceProto"
    end
    add_message "ObservationProto" do
      optional :locator, :message, 1, "LocatorProto"
      optional :observation, :message, 2, "InstanceProto"
    end
    add_message "RewardProto" do
      optional :agent, :string, 1
      optional :reward, :double, 2
    end
    add_message "InfoProto" do
      optional :agent, :string, 1
      repeated :infos, :message, 2, "InstanceProto"
    end
    add_message "GymizeProto" do
      repeated :reset_agents, :string, 1
      repeated :request_agents, :string, 2
      repeated :response_agents, :string, 3
      repeated :actions, :message, 4, "ActionProto"
      repeated :observations, :message, 5, "ObservationProto"
      repeated :rewards, :message, 6, "RewardProto"
      repeated :terminated_agents, :string, 7
      repeated :truncated_agents, :string, 8
      repeated :infos, :message, 9, "InfoProto"
      optional :rendering, :message, 10, "RenderProto"
    end
  end
end

ActionProto = ::Google::Protobuf::DescriptorPool.generated_pool.lookup("ActionProto").msgclass
ObservationProto = ::Google::Protobuf::DescriptorPool.generated_pool.lookup("ObservationProto").msgclass
RewardProto = ::Google::Protobuf::DescriptorPool.generated_pool.lookup("RewardProto").msgclass
InfoProto = ::Google::Protobuf::DescriptorPool.generated_pool.lookup("InfoProto").msgclass
GymizeProto = ::Google::Protobuf::DescriptorPool.generated_pool.lookup("GymizeProto").msgclass
