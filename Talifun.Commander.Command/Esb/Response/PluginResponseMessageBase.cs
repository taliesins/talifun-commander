using System;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Response
{
	[Serializable]
	public abstract class PluginResponseMessageBase : CorrelatedMessageBase<IPluginResponseMessage>, IPluginResponseMessage
	{
		public Guid ParentCorrelationId { get; set; }
		public string InputFilePath { get; set; }
		public FileMatchElement FileMatch { get; set; }
	}
}
