using System;

namespace Talifun.Commander.Command.Esb
{
	public class ParallelWorkflowStep<T> 
		where T : class
	{
		public Guid CorrelationId { get; set; }
		public bool Executed { get; set; }
		public T MessageInput { get; set; }
	}
}
