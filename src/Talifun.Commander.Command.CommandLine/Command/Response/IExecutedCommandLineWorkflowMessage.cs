using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.CommandLine.Command.Response
{
	public interface IExecutedCommandLineWorkflowMessage : ICommandIdentifier
	{
		bool EncodeSuccessful { get; set; }
		string OutPutFilePath { get; set; }
		string Output { get; set; }
	}
}
