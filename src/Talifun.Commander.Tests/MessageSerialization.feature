Feature: Message Serialization
	In order to publish messages to the service bus
	As a service bus user
	I want to be able to serialize the message

Scenario: Serialize Cancel Command Message
	Given a "Cancel Command" message
	When the message is serialized
	And the message is deserialized
	Then the result should be a matching message

Scenario: Serialize Request Command Message
	Given a "Request Command" message
	When the message is serialized
	And the message is deserialized
	Then the result should be a matching message

Scenario: Serialize Request Command Configuration Test Message
	Given a "Request Command Configuration Test" message
	When the message is serialized
	And the message is deserialized
	Then the result should be a matching message