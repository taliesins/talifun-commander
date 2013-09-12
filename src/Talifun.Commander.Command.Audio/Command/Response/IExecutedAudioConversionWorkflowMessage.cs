using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio.Command.Response
{
	public interface IExecutedAudioConversionWorkflowMessage : ICommandIdentifier
	{
		bool EncodeSuccessful { get; set; }
		string OutPutFilePath { get; set; }
		string Output { get; set; }
	}
}
