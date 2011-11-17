using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Request
{
	[Serializable]
	public abstract class CommandConfigurationTestRequestMessageBase : CorrelatedMessageBase<ICommandConfigurationTestRequestMessage>, ICommandConfigurationTestRequestMessage
	{
		public ProjectElement Project { get; set; }
		public Dictionary<string, string> AppSettings { get; set; }
	}
}
