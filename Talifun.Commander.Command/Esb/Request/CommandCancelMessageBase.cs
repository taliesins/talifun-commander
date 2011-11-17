using System;

namespace Talifun.Commander.Command.Esb.Request
{
	[Serializable]
	public abstract class CommandCancelMessageBase : CorrelatedMessageBase<ICommandCancelMessage>, ICommandCancelMessage
	{
	}
}
