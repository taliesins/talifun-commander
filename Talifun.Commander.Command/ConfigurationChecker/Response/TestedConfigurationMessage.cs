using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.ConfigurationChecker.Response
{
	[Serializable]
	public class TestedConfigurationMessage : CorrelatedMessageBase<TestedConfigurationMessage>
	{
		public IList<Exception> Exceptions { get; set; }
	}
}
