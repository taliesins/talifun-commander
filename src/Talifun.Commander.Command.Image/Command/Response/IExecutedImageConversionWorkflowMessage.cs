using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Image.Command.Response
{
	public interface IExecutedImageConversionWorkflowMessage : ICommandIdentifier
	{
		bool EncodeSuccessful { get; set; }
		string OutPutFilePath { get; set; }
		string Output { get; set; }
	}
}
