using System;

namespace Talifun.Commander.Command.Esb.Events
{
	[Serializable]
	public abstract class CommandCompletedMessageBase : ICommandCompletedMessage
	{
		public Guid CorrelationId { get; set; }

		public bool Equals(ICommandCompletedMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ICommandCompletedMessage)) return false;
			return Equals((ICommandCompletedMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
