using System.ComponentModel.Composition;
using MassTransit;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface ICommandConfigurationTestRequestMessageHandler<T> : Consumes<T>.All where T : class, ICommandConfigurationTestRequestMessage
	{
	}
}
