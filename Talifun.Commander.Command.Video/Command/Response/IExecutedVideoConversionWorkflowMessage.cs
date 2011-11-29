using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Video.Command.Response
{
	public interface IExecutedVideoConversionWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		bool EncodeSuccessful { get; set; }
		string OutPutFilePath { get; set; }
		string Output { get; set; }
	}
}
