using System;
using MassTransit;
using MassTransit.BusConfigurators;

namespace Talifun.Commander.Command.Esb
{
	public class ServiceInstance : IDisposable
	{
		volatile bool _disposed;

		public ServiceInstance(string name, string subscriptionServiceUri, Action<ServiceBusConfigurator> configurator)
		{
			DataBus = ServiceBusFactory.New(x =>
			{
				x.UseJsonSerializer();
				x.UseSubscriptionService(subscriptionServiceUri);
				x.ReceiveFrom(name);
				x.UseControlBus();
				x.SetConcurrentConsumerLimit(1);

				configurator(x);
			});
		}

		public IServiceBus DataBus { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			DataBus.Dispose();
			DataBus = null;

			_disposed = true;
		}

		~ServiceInstance()
		{
			Dispose(false);
		}
	}
}
