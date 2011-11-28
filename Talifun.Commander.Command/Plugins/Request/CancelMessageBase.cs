using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Request
{
	[Serializable]
	public abstract class CancelMessageBase : CorrelatedMessageBase<ICancelMessage>, ICancelMessage
	{
		public Guid ParentCorrelationId { get; set; }
	}
}
