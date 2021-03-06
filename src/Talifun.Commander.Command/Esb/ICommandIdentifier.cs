﻿using System;
using MassTransit;

namespace Talifun.Commander.Command.Esb
{
	public interface ICommandIdentifier : CorrelatedBy<Guid>
	{
		new Guid CorrelationId { get; set; }
	}
}