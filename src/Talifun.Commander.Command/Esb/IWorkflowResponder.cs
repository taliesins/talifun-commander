using System;

namespace Talifun.Commander.Command.Esb
{
	public interface IWorkflowResponder
	{
		Guid ResponderCorrelationId { get; set; }
	}
}