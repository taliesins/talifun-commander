using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Request
{
	[Serializable]
	public abstract class CommandRequestMessageBase : CorrelatedMessageBase<ICommandRequestMessage>, ICommandRequestMessage
	{
		public ProjectElement Project { get; set; }
		public Dictionary<string, string> AppSettings { get; set; }
		public string WorkingFilePath { get; set; }
		public FileMatchElement FileMatch { get; set; }
	}
}
