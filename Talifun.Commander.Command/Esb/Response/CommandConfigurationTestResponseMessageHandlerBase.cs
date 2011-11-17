using System.Diagnostics;

namespace Talifun.Commander.Command.Esb.Response
{
	public abstract class CommandConfigurationTestResponseMessageHandlerBase<T> : ICommandConfigurationTestResponseMessageHandler<T> where T : class, ICommandConfigurationTestResponseMessage
	{
		public virtual void Consume(T message)
		{
			Trace.TraceInformation(message.GetType().Name);
		}
	}
}
