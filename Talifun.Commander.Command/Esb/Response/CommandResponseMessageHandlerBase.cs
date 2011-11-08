using System.Diagnostics;

namespace Talifun.Commander.Command.Esb.Response
{
	public abstract class CommandResponseMessageHandlerBase<T> : ICommandResponseMessageHandler<T> where T : class, ICommandResponseMessage
	{
		public virtual void Consume(T message)
		{
			Trace.TraceInformation(message.GetType().Name);
		}
	}
}
