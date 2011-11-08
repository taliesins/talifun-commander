using System;

namespace Talifun.Commander.Command.Esb.Response
{
	[Serializable]
	public abstract class CommandResponseMessageBase : ICommandResponseMessage
	{
		public Guid CorrelationId { get; set; }

		public bool Equals(ICommandResponseMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ICommandResponseMessage)) return false;
			return Equals((ICommandResponseMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
