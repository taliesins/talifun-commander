using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.PicasaUploader.Command.Response
{
	public interface IExecutedPicasaUploaderWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		Exception Error { get; set; }
		bool Cancelled { get; set; }
		string VideoId { get; set; }
	}
}
