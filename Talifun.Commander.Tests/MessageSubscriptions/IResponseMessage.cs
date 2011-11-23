using System;
using MassTransit;

namespace Talifun.Commander.Tests.MessageSubscriptions
{
	public interface IResponseMessage : CorrelatedBy<Guid>
	{
		string TheAnswer { get; set; }
	}
}
