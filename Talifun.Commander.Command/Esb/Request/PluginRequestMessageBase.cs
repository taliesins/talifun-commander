using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Request
{
	[Serializable]
	public abstract class PluginRequestMessageBase<T> : CorrelatedMessageBase<IPluginRequestMessage>, IPluginRequestMessage where T : NamedConfigurationElement
	{
		public Guid ParentCorrelationId { get; set; }
		public Dictionary<string, string> AppSettings { get; set; }
		public T Configuration { get; set; }
		public string InputFilePath { get; set; }
		public FileMatchElement FileMatch { get; set; }
	}
}
