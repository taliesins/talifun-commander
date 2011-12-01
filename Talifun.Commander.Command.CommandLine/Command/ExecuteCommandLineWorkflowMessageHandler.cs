using System.IO;
using MassTransit;
using Talifun.Commander.Command.CommandLine.Command.Request;
using Talifun.Commander.Command.CommandLine.Command.Response;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.CommandLine.Command
{
	public class ExecuteCommandLineWorkflowMessageHandler : ExecuteCommandLineWorkflowMessageHandlerBase, Consumes<ExecuteCommandLineWorkflowMessage>.All
	{
		public void Consume(ExecuteCommandLineWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);
			var outPutFilePath = new FileInfo(Path.Combine(message.WorkingDirectoryPath, inputFilePath.Name));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var commandPath = message.Settings.CommandPath;
			var commandArguments = message.Settings.CommandArguments.Replace("{%InputFilePath%}", inputFilePath.FullName).Replace("{%OutPutFilePath%}", outPutFilePath.FullName);

			string output;

			var encodeSuccessful = ExecuteCommandLineExecutor(message, message.WorkingDirectoryPath, commandPath, commandArguments, out output);
			
			var executedCommandLineflowMessage = new ExecutedCommandLineWorkflowMessage()
			{
				CorrelationId = message.CorrelationId,
				EncodeSuccessful = encodeSuccessful,
				OutPutFilePath = outPutFilePath.FullName,
				Output = output
			};

			var bus = BusDriver.Instance.GetBus(CommandLineService.BusName);
			bus.Publish(executedCommandLineflowMessage);
		}
	}
}
