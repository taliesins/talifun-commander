using System;
using MassTransit;

namespace Talifun.Commander.Tests.MessageSubscriptions
{
	[Serializable]
	public class RequestMessage : IEquatable<RequestMessage>, CorrelatedBy<Guid>
	{
		public Guid CorrelationId { get; set; }
		public string TheQuestion { get; set; }

		public bool Equals(RequestMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(RequestMessage)) return false;
			return Equals((RequestMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
