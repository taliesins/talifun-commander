using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.BoxNetUploader.Command.Response
{
	public interface IExecutedBoxNetUploaderWorkflowMessage : ICommandIdentifier
	{
		Exception Error { get; set; }
		bool Cancelled { get; set; }
	}
}
