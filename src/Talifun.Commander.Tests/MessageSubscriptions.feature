Feature: Message Subscriptions
	In order to communicate between bus users
	As a service bus user
	I want to send a request message and receive a response message

@ServiceBus
Scenario: Response message is received from request message when listening for response message 
	Given a request message handler
	And a response message listener
	When a request message is published
	Then a response message should be received

@ServiceBus
Scenario: Response message is received from request message when listening for response message interface
	Given a request message handler
	And a response message interface listener
	When a request message is published
	Then a response message should be received