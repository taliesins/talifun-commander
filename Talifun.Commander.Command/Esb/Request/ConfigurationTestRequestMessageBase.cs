using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Request
{
	[Serializable]
	public abstract class ConfigurationTestRequestMessageBase<T> : CorrelatedMessageBase<IConfigurationTestRequestMessage>, IConfigurationTestRequestMessage where T : CurrentConfigurationElementCollection
	{
		public Dictionary<string, string> AppSettings { get; set; }
		public string ProjectName { get; set; }
		public T Configuration { get; set; }
	}
}
