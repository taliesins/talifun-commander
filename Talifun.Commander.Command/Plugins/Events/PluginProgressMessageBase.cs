using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Events
{
	[Serializable]
	public abstract class PluginProgressMessageBase : CorrelatedMessageBase<IPluginProgressMessage>, IPluginProgressMessage
	{
		public string InputFilePath { get; set; }
		public string Output { get; set; }
	}
}
