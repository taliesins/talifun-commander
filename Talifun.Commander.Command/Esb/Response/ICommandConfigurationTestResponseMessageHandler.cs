using System.ComponentModel.Composition;
using MassTransit;

namespace Talifun.Commander.Command.Esb.Response
{
	[InheritedExport]
	public interface ICommandConfigurationTestResponseMessageHandler<T> : Consumes<T>.All where T : class, ICommandConfigurationTestResponseMessage
	{
	}
}
