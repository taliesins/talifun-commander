using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.DropBoxUploader.Command.Response
{
	public interface IExecutedDropBoxUploaderWorkflowMessage : ICommandIdentifier
	{
		Exception Error { get; set; }
		bool Cancelled { get; set; }
	}
}
