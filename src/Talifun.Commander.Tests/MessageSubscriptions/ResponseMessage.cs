using System;

namespace Talifun.Commander.Tests.MessageSubscriptions
{
	[Serializable]
	public class ResponseMessage : IEquatable<ResponseMessage>, IResponseMessage
	{
		public Guid CorrelationId { get; set; }
		public string TheAnswer { get; set; }

		public bool Equals(ResponseMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ResponseMessage)) return false;
			return Equals((ResponseMessage)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
