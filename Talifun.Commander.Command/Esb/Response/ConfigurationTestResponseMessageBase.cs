using System;

namespace Talifun.Commander.Command.Esb.Response
{
	[Serializable]
	public abstract class ConfigurationTestResponseMessageBase : CorrelatedMessageBase<IConfigurationTestResponseMessage>, IConfigurationTestResponseMessage
	{
		public Exception Exception { get; set; }
	}
}
