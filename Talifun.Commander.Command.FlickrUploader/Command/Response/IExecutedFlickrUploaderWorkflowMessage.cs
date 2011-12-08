using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FlickrUploader.Command.Response
{
	public interface IExecutedFlickrUploaderWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		Exception Error { get; set; }
		bool Cancelled { get; set; }
	}
}
