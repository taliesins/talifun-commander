using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command.Response
{
	public interface IExecutedAntiVirusWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		bool FilePassed { get; set; }
		string OutPutFilePath { get; set; }
		string Output { get; set; }
	}
}
