using System;
using System.IO;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Video.Command.AudioFormats;
using Talifun.Commander.Command.Video.Command.Request;
using Talifun.Commander.Command.Video.Command.Response;
using Talifun.Commander.Command.Video.Command.VideoFormats;
using Talifun.Commander.Command.Video.Command.Watermark;
using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.Command
{
	public class ExecuteTwoPassConversionWorkflowMessageHandler : ExecuteVideoConversionWorkflowMessageHandlerBase, Consumes<ExecuteTwoPassConversionWorkflowMessage>.All
	{
		public void Consume(ExecuteTwoPassConversionWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);

			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + "." + message.Settings.FileNameExtension;
			var outPutFilePath = new FileInfo(Path.Combine(message.WorkingDirectoryPath, fileName));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var fileLog = Path.GetFileNameWithoutExtension(inputFilePath.Name) + ".log";
			var logFilePath = new FileInfo(Path.Combine(message.WorkingDirectoryPath, fileLog));
			if (logFilePath.Exists)
			{
				logFilePath.Delete();
			}

			var output = string.Empty;

			var firstPassCommandArguments = string.Format("-i \"{0}\" -y -passlogfile \"{1}\" -pass 1 {2} {3} \"{4}\"", inputFilePath.FullName, logFilePath.FullName, message.Settings.Video.GetOptionsForFirstPass(), "-an", outPutFilePath.FullName);
			var secondPassCommandArguments = string.Format("-i \"{0}\" -y -passlogfile \"{1}\" -pass 2 {2} {3} {4} \"{5}\"", inputFilePath.FullName, logFilePath.FullName, message.Settings.Video.GetOptionsForSecondPass(), message.Settings.Audio.GetOptions(), message.Settings.Watermark.GetOptions(), outPutFilePath.FullName);

			var fFMpegCommandPath = message.AppSettings[VideoConversionConfiguration.Instance.FFMpegPathSettingName];
			
			var firstPassOutput = string.Empty;
			var secondPassOutput = string.Empty;

			var result = ExecuteFfMpegCommandLineExecutor(message, message.WorkingDirectoryPath, fFMpegCommandPath, firstPassCommandArguments, out firstPassOutput);
			output += firstPassOutput;

			if (result)
			{
				result = ExecuteFfMpegCommandLineExecutor(message, message.WorkingDirectoryPath, fFMpegCommandPath, secondPassCommandArguments, out secondPassOutput);
				output += Environment.NewLine + secondPassOutput;
			}

			var executedTwoPassWorkflowMessage = new ExecutedTwoPassConversionWorkflowMessage()
			{
				CorrelationId = message.CorrelationId,
				EncodeSuccessful = result,
				OutPutFilePath = outPutFilePath.FullName,
				Output = output
			};

			var bus = BusDriver.Instance.GetBus(VideoConversionService.BusName);
			bus.Publish(executedTwoPassWorkflowMessage);
		}
	}
}
