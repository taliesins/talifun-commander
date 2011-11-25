using System;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Events
{
	[Serializable]
	public abstract class CommandProgressMessageBase : CorrelatedMessageBase<ICommandProgressMessage>, ICommandProgressMessage
	{
		public string InputFilePath { get; set; }
		public FileMatchElement FileMatch { get; set; }
	}
}
