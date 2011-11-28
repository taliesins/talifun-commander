using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio.Command.Response
{
	public interface IExecutedAudioConversionWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		bool EncodeSuccessful { get; set; }
		string OutPutFilePath { get; set; }
		string Output { get; set; }
	}
}
