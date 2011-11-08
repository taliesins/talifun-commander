using System;
using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.Services.Subscriptions.Server;

namespace Talifun.Commander.Command.Esb
{
	public interface IBusDriver
	{
		SubscriptionService SubscriptionService { get; }
		IServiceBus AddBus(string instanceName, string queueName, Action<ServiceBusConfigurator> configureBus);
		IServiceBus GetBus(string instanceName);
		void RemoveBus(string instanceName);
	}
}
