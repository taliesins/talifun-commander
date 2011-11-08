using System.ComponentModel.Composition;
using MassTransit;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface ICommandCancelMessageHandler<T> : Consumes<T>.All where T : class, ICommandCancelMessage 
	{
	}
}
