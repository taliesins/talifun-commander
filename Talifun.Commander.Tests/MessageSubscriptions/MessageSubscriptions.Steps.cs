using System;
using MassTransit;
using MassTransit.RequestResponse.Configurators;
using Talifun.Commander.Command.Esb;
using TechTalk.SpecFlow;
using FluentAssertions;

namespace Talifun.Commander.Tests.MessageSubscriptions
{
	[Binding]
	public class MessageSubscriptions
	{
		public const string RequesterName = "Requester";
		public const string ResponderName = "Responder";

		private IServiceBus _requesterBus;
		private IServiceBus _responderBus;

		private Action<RequestConfigurator<RequestMessage>> _listener;

		private bool _responseMessageReceived;

		[AfterScenario("ServiceBus")]
		public void AfterScenario()
		{
			_responderBus = null;
			_requesterBus = null;
			BusDriver.Instance.RemoveBus(ResponderName);
			BusDriver.Instance.RemoveBus(RequesterName);
		}

		[Given(@"a request message handler")]
		public void GivenARequestMessageHandler()
		{
			_responderBus = BusDriver.Instance.AddBus(ResponderName, string.Format("loopback://localhost/{0}", ResponderName), x =>
			{
				x.Subscribe(subscriber =>
				{
					subscriber.Consumer<RequestMessageHandler>();
				});
			});
		}

		[Given(@"a response message listener")]
		public void GivenAResponseMessageListener()
		{
			_requesterBus = BusDriver.Instance.AddBus(RequesterName, string.Format("loopback://localhost/{0}", RequesterName), x =>
			{
			});

			_listener = x =>
			{
				x.Handle<ResponseMessage>(message =>
				{
					_responseMessageReceived = true;
				});
			};
		}

		[Given(@"a response message interface listener")]
		public void GivenAResponseMessageInterfaceListener()
		{
			_requesterBus = BusDriver.Instance.AddBus(RequesterName, string.Format("loopback://localhost/{0}", RequesterName), x =>
			{
			});

			_listener = x =>
			{
				x.Handle<IResponseMessage>(message =>
				{
					_responseMessageReceived = true;
				});
			};
		}

		[When(@"a request message is published")]
		public void WhenARequestMessageIsPublished()
		{
			var requestMessage = new RequestMessage
			{
				CorrelationId = Guid.NewGuid(),
				TheQuestion = "Question"
			};

			_requesterBus.PublishRequest(requestMessage, _listener);
		}

		[Then(@"a response message should be received")]
		public void ThenAResponseMessageShouldBeReceived()
		{
			_responseMessageReceived.Should().BeTrue();
		}
	}
}
