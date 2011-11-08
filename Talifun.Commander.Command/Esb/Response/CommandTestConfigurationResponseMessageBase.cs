using System;

namespace Talifun.Commander.Command.Esb.Response
{
	[Serializable]
	public abstract class CommandTestConfigurationResponseMessageBase : ICommandTestConfigurationResponseMessage
	{
		public Guid CorrelationId { get; set; }

		public bool Equals(ICommandTestConfigurationResponseMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ICommandTestConfigurationResponseMessage)) return false;
			return Equals((ICommandTestConfigurationResponseMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
