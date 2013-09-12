using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Video.Command.Response
{
	public interface IExecutedVideoConversionWorkflowMessage : ICommandIdentifier
	{
		bool EncodeSuccessful { get; set; }
		string OutPutFilePath { get; set; }
		string Output { get; set; }
	}
}
