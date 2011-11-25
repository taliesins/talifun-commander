using System.ComponentModel.Composition;
using MassTransit;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface ICancelMessageHandler<T> : Consumes<T>.All where T : class, ICancelMessage 
	{
	}
}
