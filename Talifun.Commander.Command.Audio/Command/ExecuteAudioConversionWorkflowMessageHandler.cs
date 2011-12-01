using System;
using System.IO;
using MassTransit;
using Talifun.Commander.Command.Audio.Command.AudioFormats;
using Talifun.Commander.Command.Audio.Command.Request;
using Talifun.Commander.Command.Audio.Command.Response;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio.Command
{
	public class ExecuteAudioConversionWorkflowMessageHandler : ExecuteAudioConversionWorkflowMessageHandlerBase, Consumes<ExecuteAudioConversionWorkflowMessage>.All
	{
		public void Consume(ExecuteAudioConversionWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);

			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + "." + message.Settings.FileNameExtension;
			var outPutFilePath = new FileInfo(Path.Combine(message.WorkingDirectoryPath, fileName));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var commandPath = message.AppSettings[AudioConversionConfiguration.Instance.FFMpegPathSettingName];
			var commandArguments = String.Format("-i \"{0}\" -y {1} \"{2}\"", inputFilePath.FullName, message.Settings.GetOptions(), outPutFilePath.FullName);

			string output;

			var encodeSuccessful = ExecuteFfMpegCommandLineExecutor(message, message.WorkingDirectoryPath, commandPath, commandArguments, out output);

			var executedMcAfeeWorkflowMessage = new ExecutedAudioConversionWorkflowMessage()
			{
				CorrelationId = message.CorrelationId,
				EncodeSuccessful = encodeSuccessful,
				OutPutFilePath = outPutFilePath.FullName,
				Output = output
			};

			var bus = BusDriver.Instance.GetBus(AudioConversionService.BusName);
			bus.Publish(executedMcAfeeWorkflowMessage);
		}
	}
}
