using System;
using MassTransit;

namespace Talifun.Commander.Command.TestConfiguration
{
	[Serializable]
	public class ResponseTestConfigurationMessage : IEquatable<ResponseTestConfigurationMessage>, CorrelatedBy<Guid>
	{
		public Guid CorrelationId { get; set; }

		public bool Equals(ResponseTestConfigurationMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ResponseTestConfigurationMessage)) return false;
			return Equals((ResponseTestConfigurationMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
