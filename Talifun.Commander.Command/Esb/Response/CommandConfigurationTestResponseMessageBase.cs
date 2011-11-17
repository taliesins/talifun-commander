using System;

namespace Talifun.Commander.Command.Esb.Response
{
	[Serializable]
	public abstract class CommandConfigurationTestResponseMessageBase : CorrelatedMessageBase<ICommandConfigurationTestResponseMessage>, ICommandConfigurationTestResponseMessage
	{
		public Exception Exception { get; set; }
	}
}
