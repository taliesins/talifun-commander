using System;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Response
{
	[Serializable]
	public abstract class CommandResponseMessageBase : CorrelatedMessageBase<ICommandResponseMessage>, ICommandResponseMessage
	{
		public string InputFilePath { get; set; }
		public FileMatchElement FileMatch { get; set; }
	}
}
