using System;
using System.Collections.Generic;
using Magnum.Extensions;
using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.Configurators;
using MassTransit.EndpointConfigurators;
using MassTransit.Exceptions;
using MassTransit.Saga;
using MassTransit.Services.Subscriptions.Server;
using MassTransit.Transports;
using MassTransit.Transports.Loopback;

namespace Talifun.Commander.Command.Esb
{
	public sealed class BusDriver : IBusDriver, IDisposable
	{
		private BusDriver()
		{
			var defaultSettings = new EndpointFactoryDefaultSettings();

			_endpointFactoryConfigurator = new EndpointFactoryConfiguratorImpl(defaultSettings);
			_endpointFactoryConfigurator.AddTransportFactory<LoopbackTransportFactory>();
			_endpointFactoryConfigurator.SetPurgeOnStartup(true);

			Buses = new Dictionary<string, ServiceInstance>();

			SetupEndPointCache();
			SetupSubscriptionService();
		}

		public static readonly IBusDriver Instance = new BusDriver();

		#region Endpoint Cache
		EndpointFactoryConfiguratorImpl _endpointFactoryConfigurator;
		EndpointCache _endpointCache;
		private IEndpointFactory EndpointFactory { get; set; }
		private IEndpointCache EndpointCache { get; set; }

		private void SetupEndPointCache()
		{
			if (_endpointFactoryConfigurator != null)
			{
				var result = ConfigurationResultImpl.CompileResults(_endpointFactoryConfigurator.Validate());

				try
				{
					EndpointFactory = _endpointFactoryConfigurator.CreateEndpointFactory();
					_endpointFactoryConfigurator = null;

					_endpointCache = new EndpointCache(EndpointFactory);

					EndpointCache = new EndpointCacheProxy(_endpointCache);
				}
				catch (Exception ex)
				{
					throw new ConfigurationException(result, "An exception was thrown during endpoint cache creation", ex);
				}
			}

			ServiceBusFactory.ConfigureDefaultSettings(x =>
			{
				x.SetEndpointCache(EndpointCache);
				x.SetConcurrentConsumerLimit(1);
				x.SetReceiveTimeout(50.Milliseconds());
				x.EnableAutoStart();
			});
		}
		#endregion

		#region SubscriptionService
		private const string SubscriptionServiceUri = "loopback://localhost/mt_subscriptions";
		private ISagaRepository<SubscriptionClientSaga> _subscriptionClientSagaRepository;
		private ISagaRepository<SubscriptionSaga> _subscriptionSagaRepository;
		private IServiceBus SubscriptionBus { get; set; }
		public SubscriptionService SubscriptionService { get; private set; }

		private void SetupSubscriptionService()
		{
			SubscriptionBus = ServiceBusFactory.New(x =>
			{
				x.ReceiveFrom(SubscriptionServiceUri);
				x.SetConcurrentConsumerLimit(1);
			});

			_subscriptionClientSagaRepository = SetupSagaRepository<SubscriptionClientSaga>();
			_subscriptionSagaRepository = SetupSagaRepository<SubscriptionSaga>();
			SubscriptionService = new SubscriptionService(SubscriptionBus, _subscriptionSagaRepository, _subscriptionClientSagaRepository);
			SubscriptionService.Start();
		}

		private static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>() where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();
			return sagaRepository;
		}
		#endregion

		#region Buses
		private Dictionary<string, ServiceInstance> Buses { get; set; }
		public IServiceBus AddBus(string instanceName, string queueName, Action<ServiceBusConfigurator> configureBus)
		{
			var bus = new ServiceInstance(queueName, SubscriptionServiceUri, configureBus);
			Buses.Add(instanceName, bus);
			return bus.DataBus;
		}

		public IServiceBus GetBus(string instanceName)
		{
			ServiceInstance bus;
			if (!Buses.TryGetValue(instanceName, out bus)) return null;
			return bus.DataBus;
		}

		public void RemoveBus(string instanceName)
		{
			ServiceInstance bus;
			if (!Buses.TryGetValue(instanceName, out bus)) return;
			Buses.Remove(instanceName);
			bus.Dispose();
		}
		#endregion

		volatile bool _disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			_endpointCache.Clear();

			_disposed = true;
		}

		~BusDriver()
		{
			Dispose(false);
		}
	}
}
