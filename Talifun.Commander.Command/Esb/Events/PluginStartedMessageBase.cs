using System;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Events
{
	[Serializable]
	public abstract class PluginStartedMessageBase : CorrelatedMessageBase<IPluginStartedMessage>, IPluginStartedMessage
	{
		public string InputFilePath { get; set; }
		public FileMatchElement FileMatch { get; set; }
	}
}
