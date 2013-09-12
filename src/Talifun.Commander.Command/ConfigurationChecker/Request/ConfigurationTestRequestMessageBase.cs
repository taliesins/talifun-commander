using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.ConfigurationChecker.Request
{
	[Serializable]
	public abstract class ConfigurationTestRequestMessageBase<T> : CorrelatedMessageBase<IConfigurationTestRequestMessage>, IConfigurationTestRequestMessage where T : CurrentConfigurationElementCollection
	{
		public Guid RequestorCorrelationId { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string ProjectName { get; set; }
		public T Configuration { get; set; }
	}
}
