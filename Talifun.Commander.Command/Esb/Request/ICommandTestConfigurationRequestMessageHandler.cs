using System.ComponentModel.Composition;
using MassTransit;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface ICommandTestConfigurationRequestMessageHandler<T> : Consumes<T>.All where T : class, ICommandTestConfigurationRequestMessage
	{
	}
}
