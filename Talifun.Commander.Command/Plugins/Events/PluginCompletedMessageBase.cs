using System;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Events
{
	[Serializable]
	public abstract class PluginCompletedMessageBase : CorrelatedMessageBase<IPluginCompletedMessage>, IPluginCompletedMessage
	{
		public string InputFilePath { get; set; }
		public FileMatchElement FileMatch { get; set; }
	}
}
