using System;

namespace Talifun.Commander.Command.Esb
{
	[Serializable]
	public abstract class CorrelatedMessageBase<T> : ICommandIdentifier, IEquatable<T> where T : ICommandIdentifier
	{
		public Guid CorrelationId { get; set; }

		public bool Equals(T obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(T)) return false;
			return Equals((T)obj);
		}

		public override int GetHashCode()
		{
			return CorrelationId.GetHashCode();
		}
	}
}
