using System.IO;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Video.Command.AudioFormats;
using Talifun.Commander.Command.Video.Command.Request;
using Talifun.Commander.Command.Video.Command.Response;
using Talifun.Commander.Command.Video.Command.VideoFormats;
using Talifun.Commander.Command.Video.Command.Watermark;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Video.Command
{
	public class ExecuteOnePassConversionWorkflowMessageHandler : Consumes<ExecuteOnePassConversionWorkflowMessage>.All
	{
		public void Consume(ExecuteOnePassConversionWorkflowMessage message)
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

			var ffmpegHelper = new FfMpegCommandLineExecutor();
			var result = ffmpegHelper.Execute(message.WorkingDirectoryPath, fFMpegCommandPath, fFMpegCommandArguments, out encodeOutput);
			output = encodeOutput;

			var executedOnePassWorkflowMessage = new ExecutedOnePassConversionWorkflowMessage()
			{
				CorrelationId = message.CorrelationId,
				EncodeSuccessful = result,
				OutPutFilePath = outPutFilePath.FullName,
				Output = output
			};

			var bus = BusDriver.Instance.GetBus(VideoConversionService.BusName);
			bus.Publish(executedOnePassWorkflowMessage);
		}
	}
}
