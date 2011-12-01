using System;
using System.IO;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.Request;
using Talifun.Commander.Command.Image.Command.Response;
using Talifun.Commander.Command.Image.Configuration;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.Image.Command
{
	public class ExecuteImageConversionWorkflowMessageHandler : ExecuteImageConversionWorkflowMessageHandlerBase, Consumes<ExecuteImageConversionWorkflowMessage>.All
	{
		public void Consume(ExecuteImageConversionWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);

			var extension = "";
			switch (message.Settings.ResizeImageType)
			{
				case ResizeImageType.JPG:
					extension = ".jpg";
					break;
				case ResizeImageType.PNG:
					extension = ".png";
					break;
				case ResizeImageType.GIF:
					extension = ".gif";
					break;
				default:
					extension = Path.GetExtension(inputFilePath.Name);
					break;
			}

			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + extension;
			var outPutFilePath = new FileInfo(Path.Combine(message.WorkingDirectoryPath, fileName));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var output = string.Empty;

			var convertPath = message.AppSettings[ImageConversionConfiguration.Instance.ConvertPathSettingName];
			var convertArguments = ThumbnailArguments(message.Settings, inputFilePath.FullName, outPutFilePath.FullName);
			var convertOutput = string.Empty;

			var result = ExecuteCommandLineExecutor(message, message.WorkingDirectoryPath, convertPath, convertArguments, out convertOutput);
			output += convertOutput;

			if (result && !string.IsNullOrEmpty(message.Settings.WatermarkPath))
			{
				var compositePath = message.AppSettings[ImageConversionConfiguration.Instance.CompositePathSettingName];
				var compositeArguments = WatermarkArguments(message.Settings, outPutFilePath.FullName);
				var compositeOutput = string.Empty;

				result = ExecuteCommandLineExecutor(message, message.WorkingDirectoryPath, compositePath, compositeArguments, out compositeOutput);

				output += Environment.NewLine + compositeOutput;
			}

			var executedMcAfeeWorkflowMessage = new ExecutedImageConversionWorkflowMessage()
			{
				CorrelationId = message.CorrelationId,
				EncodeSuccessful = result,
				OutPutFilePath = outPutFilePath.FullName,
				Output = output
			};

			var bus = BusDriver.Instance.GetBus(ImageConversionService.BusName);
			bus.Publish(executedMcAfeeWorkflowMessage);
		}
	}
}
