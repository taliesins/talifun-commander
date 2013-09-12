using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Request
{
	[Serializable]
	public abstract class PluginRequestMessageBase<T> : CorrelatedMessageBase<IPluginRequestMessage>, IPluginRequestMessage where T : NamedConfigurationElement
	{
		public Guid RequestorCorrelationId { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public T Configuration { get; set; }
		public string InputFilePath { get; set; }
		public FileMatchElement FileMatch { get; set; }
	}
}
