using System.ComponentModel.Composition;
using MassTransit.BusConfigurators;

namespace Talifun.Commander.Command
{
	[InheritedExport]
	public interface ICommandService
	{
		void Start();
		void ConfigureCommandServiceBus(ServiceBusConfigurator serviceBusConfigurator);
		void Stop();
	}
}