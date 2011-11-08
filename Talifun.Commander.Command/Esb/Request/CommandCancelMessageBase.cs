using System;

namespace Talifun.Commander.Command.Esb.Request
{
	[Serializable]
	public abstract class CommandCancelMessageBase : ICommandCancelMessage
	{
		public Guid CorrelationId { get; set; }

		public bool Equals(ICommandCancelMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ICommandCancelMessage)) return false;
			return Equals((ICommandCancelMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
