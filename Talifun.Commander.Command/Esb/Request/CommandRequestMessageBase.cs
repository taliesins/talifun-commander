using System;

namespace Talifun.Commander.Command.Esb.Request
{
	[Serializable]
	public abstract class CommandRequestMessageBase : CorrelatedMessageBase<ICommandRequestMessage>, ICommandRequestMessage
	{
	}
}
