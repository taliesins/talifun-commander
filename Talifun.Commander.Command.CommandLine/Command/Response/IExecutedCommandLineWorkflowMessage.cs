using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.CommandLine.Command.Response
{
	public interface IExecutedCommandLineWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		bool EncodeSuccessful { get; set; }
		string OutPutFilePath { get; set; }
		string Output { get; set; }
	}
}
