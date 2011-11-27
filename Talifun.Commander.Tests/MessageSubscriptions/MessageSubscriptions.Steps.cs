using System;
using System.Threading;
using Magnum;
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

		[BeforeScenario("ServiceBus")]
		public void BeforeScenario()
		{
			_requesterBus = BusDriver.Instance.AddBus(RequesterName, string.Format("loopback://localhost/{0}", RequesterName), x =>
			{
			});

			_responderBus = BusDriver.Instance.AddBus(ResponderName, string.Format("loopback://localhost/{0}", ResponderName), x =>
			{
			});

		}

		[AfterScenario("ServiceBus")]
		public void AfterScenario()
		{
			if (_responderBus != null)
			{
				_responderBus = null;
				BusDriver.Instance.RemoveBus(ResponderName);	
			}

			if (_requesterBus != null)
			{
				_requesterBus = null;
				BusDriver.Instance.RemoveBus(RequesterName);
			}

			BusDriver.Instance.SubscriptionService.Stop();
			BusDriver.Instance.SubscriptionService.Start();
		}

		[Given(@"a request message handler")]
		public void GivenARequestMessageHandler()
		{
			_responderBus.SubscribeConsumer<RequestMessageHandler>();
			Thread.Sleep(50);
			Thread.Yield();
			Thread.Sleep(50);
			Thread.Yield();
		}

		[Given(@"a response message listener")]
		public void GivenAResponseMessageListener()
		{
			_listener = x =>
			{
				x.Handle<ResponseMessage>(message =>
				{
					_responseMessageReceived = true;
				});
				x.SetTimeout(new TimeSpan(0,0,0,5));
			};
		}

		[Given(@"a response message interface listener")]
		public void GivenAResponseMessageInterfaceListener()
		{
			_listener = x =>
			{
				x.Handle<IResponseMessage>(message =>
				{
					_responseMessageReceived = true;
				});
				x.SetTimeout(new TimeSpan(0, 0, 0, 5));
			};
		}

		[When(@"a request message is published")]
		public void WhenARequestMessageIsPublished()
		{
			var requestMessage = new RequestMessage
			{
				CorrelationId = CombGuid.Generate(),
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
