using System;

namespace Talifun.Commander.Command.Esb.Response
{
	[Serializable]
	public abstract class CommandResponseMessageBase : CorrelatedMessageBase<ICommandResponseMessage>, ICommandResponseMessage
	{
	}
}
