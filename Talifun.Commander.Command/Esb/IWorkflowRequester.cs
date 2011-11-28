using System;

namespace Talifun.Commander.Command.Esb
{
	public interface IWorkflowRequester
	{
		Guid RequestorCorrelationId { get; }
	}
}
