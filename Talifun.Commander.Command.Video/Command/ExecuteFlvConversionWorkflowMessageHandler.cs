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
	public class ExecuteFlvConversionWorkflowMessageHandler : ExecuteVideoConversionWorkflowMessageHandlerBase, Consumes<ExecuteFlvConversionWorkflowMessage>.All
	{
		public void Consume(ExecuteFlvConversionWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);

			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + "." + message.Settings.FileNameExtension;
			var outPutFilePath = new FileInfo(Path.Combine(message.WorkingDirectoryPath, fileName));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var output = string.Empty;
			var fFMpegCommandPath = message.AppSettings[VideoConversionConfiguration.Instance.FFMpegPathSettingName];
			var fFMpegCommandArguments = string.Format("-i \"{0}\" -y {1} {2} {3} \"{4}\"", inputFilePath.FullName, message.Settings.Video.GetOptionsForFirstPass(), message.Settings.Audio.GetOptions(), message.Settings.Watermark.GetOptions(), outPutFilePath.FullName);
			
			var encodeOutput = string.Empty;
			var result = ExecuteFfMpegCommandLineExecutor(message, message.WorkingDirectoryPath, fFMpegCommandPath, fFMpegCommandArguments, out encodeOutput);
			
			output = encodeOutput;

			if (result)
			{
				var flvTool2CommandArguments = string.Format("-U \"{0}\"", outPutFilePath.Name);
				var flvTool2CommandPath = message.AppSettings[VideoConversionConfiguration.Instance.FlvTool2PathSettingName];
				var flvTool2Output = string.Empty;

				result = ExecuteCommandLineExecutor(message, message.WorkingDirectoryPath, flvTool2CommandPath, flvTool2CommandArguments, out flvTool2Output);
				output += Environment.NewLine + flvTool2Output;
			}

			var executedFlvWorkflowMessage = new ExecutedFlvConversionWorkflowMessage()
			{
				CorrelationId = message.CorrelationId,
				EncodeSuccessful = result,
				OutPutFilePath = outPutFilePath.FullName,
				Output = output
			};

			var bus = BusDriver.Instance.GetBus(VideoConversionService.BusName);
			bus.Publish(executedFlvWorkflowMessage);
		}
	}
}
