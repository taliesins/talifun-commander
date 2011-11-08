using System;

namespace Talifun.Commander.Command.Esb.Events
{
	[Serializable]
	public abstract class CommandStartedMessageBase : ICommandStartedMessage
	{
		public Guid CorrelationId { get; set; }

		public bool Equals(ICommandStartedMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ICommandStartedMessage)) return false;
			return Equals((ICommandStartedMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
