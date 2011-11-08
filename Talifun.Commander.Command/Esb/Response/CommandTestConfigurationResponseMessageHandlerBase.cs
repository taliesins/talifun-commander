using System.Diagnostics;

namespace Talifun.Commander.Command.Esb.Response
{
	public abstract class CommandTestConfigurationResponseMessageHandlerBase<T> : ICommandTestConfigurationResponseMessageHandler<T> where T : class, ICommandTestConfigurationResponseMessage
	{
		public virtual void Consume(T message)
		{
			Trace.TraceInformation(message.GetType().Name);
		}
	}
}
