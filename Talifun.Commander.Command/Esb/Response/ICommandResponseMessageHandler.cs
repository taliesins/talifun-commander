using System.ComponentModel.Composition;
using MassTransit;

namespace Talifun.Commander.Command.Esb.Response
{
	[InheritedExport]
	public interface ICommandResponseMessageHandler<T> : Consumes<T>.All where T : class, ICommandResponseMessage
	{
	}
}
