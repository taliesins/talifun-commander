using System;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Events
{
	[Serializable]
	public abstract class PluginStartedMessageBase : CorrelatedMessageBase<IPluginStartedMessage>, IPluginStartedMessage
	{
		public string InputFilePath { get; set; }
		public FileMatchElement FileMatch { get; set; }
	}
}
