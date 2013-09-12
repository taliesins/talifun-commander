using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Response
{
	[Serializable]
	public abstract class PluginResponseMessageBase : CorrelatedMessageBase<IPluginResponseMessage>, IPluginResponseMessage
	{
		public Guid ResponderCorrelationId { get; set; }
	}
}
