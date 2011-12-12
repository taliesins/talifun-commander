using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.BoxNetUploader.Command.Response
{
	public interface IExecutedBoxNetUploaderWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		Exception Error { get; set; }
		bool Cancelled { get; set; }
	}
}
