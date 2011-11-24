using System;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Events
{
	[Serializable]
	public abstract class CommandCompletedMessageBase : CorrelatedMessageBase<ICommandCompletedMessage>, ICommandCompletedMessage
	{
		public string WorkingFilePath { get; set; }
		public FileMatchElement FileMatch { get; set; }
	}
}
