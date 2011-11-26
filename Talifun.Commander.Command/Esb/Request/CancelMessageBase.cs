using System;

namespace Talifun.Commander.Command.Esb.Request
{
	[Serializable]
	public abstract class CancelMessageBase : CorrelatedMessageBase<ICancelMessage>, ICancelMessage
	{
		public Guid ParentCorrelationId { get; set; }
	}
}
