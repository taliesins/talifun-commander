using System;

namespace Talifun.Commander.Command.Esb.Request
{
	[Serializable]
	public abstract class CommandTestConfigurationRequestMessageBase : ICommandTestConfigurationRequestMessage
	{
		public Guid CorrelationId { get; set; }

		public bool Equals(ICommandTestConfigurationRequestMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ICommandTestConfigurationRequestMessage)) return false;
			return Equals((ICommandTestConfigurationRequestMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
