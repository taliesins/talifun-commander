using System;

namespace Talifun.Commander.Command.Esb.Events
{
	[Serializable]
	public abstract class CommandProgressMessageBase : ICommandProgressMessage
	{
		public Guid CorrelationId { get; set; }

		public bool Equals(ICommandProgressMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ICommandProgressMessage)) return false;
			return Equals((ICommandProgressMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
