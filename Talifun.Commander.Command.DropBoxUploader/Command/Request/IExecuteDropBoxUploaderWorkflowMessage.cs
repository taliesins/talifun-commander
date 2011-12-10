using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.DropBoxUploader.Command.Settings;

namespace Talifun.Commander.Command.DropBoxUploader.Command.Request
{
	public interface IExecuteDropBoxUploaderWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		IDropBoxUploaderSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
	}
}
