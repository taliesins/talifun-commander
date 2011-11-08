using System;
using MassTransit;

namespace Talifun.Commander.Command.TestConfiguration
{
	[Serializable]
	public class RequestTestConfigurationMessage : IEquatable<RequestTestConfigurationMessage>, CorrelatedBy<Guid>
	{
		public Guid CorrelationId { get; set; }

		public bool Equals(RequestTestConfigurationMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(RequestTestConfigurationMessage)) return false;
			return Equals((RequestTestConfigurationMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
