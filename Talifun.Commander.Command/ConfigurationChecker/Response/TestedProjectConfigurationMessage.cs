using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.ConfigurationChecker.Response
{
	[Serializable]
	public class TestedProjectConfigurationMessage : CorrelatedMessageBase<TestedProjectConfigurationMessage>, IWorkflowResponder
	{
		public Guid ResponderCorrelationId { get; set; }
		public IList<Exception> Exceptions { get; set; }
	}
}