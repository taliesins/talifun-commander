using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.ConfigurationChecker.Request
{
	[Serializable]
	public class TestProjectConfigurationMessage : CorrelatedMessageBase<TestProjectConfigurationMessage>, IWorkflowRequester
	{
		public Guid RequestorCorrelationId { get; set; }
		public ProjectElement Project { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
	}
}
