using System;
using System.IO;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.VideoThumbNailer.Command.Request;
using Talifun.Commander.Command.VideoThumbNailer.Command.Response;
using Talifun.Commander.Command.VideoThumbnailer;
using Talifun.Commander.Command.VideoThumbnailer.Command;
using Talifun.Commander.Command.VideoThumbnailer.Configuration;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.VideoThumbNailer.Command
{
	public class ExecuteVideoThumbnailerWorkflowMessageHandler : ExecuteVideoThumbnailerWorkflowMessageHandlerBase, Consumes<ExecuteVideoThumbnailerWorkflowMessage>.All
	{
		const string AllFixedOptions = @"-f image2 -vframes 1";

		public void Consume(ExecuteVideoThumbnailerWorkflowMessage message)
		{
			var extension = "";

			switch (message.Settings.ImageType)
			{
				case ImageType.JPG:
					extension = ".jpg";
					break;
				case ImageType.PNG:
					extension = ".png";
					break;
			}

			var inputFilePath = new FileInfo(message.InputFilePath);

			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + extension;
			var outPutFilePath = new FileInfo(Path.Combine(message.WorkingDirectoryPath, fileName));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var thumbnailCreationSuccessful = true;
			var output = string.Empty;
			var position = string.Empty;

			var commandPath = message.AppSettings[VideoThumbnailerConfiguration.Instance.FFMpegPathSettingName];
			var videoInfoOutput = string.Empty;
			if (message.Settings.TimePercentage >= 0 && message.Settings.TimePercentage <= 100)
			{
				var videoInfo = VideoInfo.GetVideoInfo(commandPath, inputFilePath, out videoInfoOutput);

				if (videoInfo == null)
				{
					output = videoInfoOutput;
					thumbnailCreationSuccessful = false;
				}
				else
				{
					var seconds = Convert.ToInt32(Math.Truncate(videoInfo.Duration.TotalSeconds*message.Settings.TimePercentage)/100);
					var duration = new TimeSpan(0, 0, 0, seconds);

					position = string.Format("-ss {0}", duration.ToString());
				}
			}
			else if (message.Settings.Time != TimeSpan.Zero)
			{
				position = string.Format("-ss {0}", message.Settings.Time.ToString());
			}

			if (thumbnailCreationSuccessful)
			{
				var commandArguments = string.Format("-i \"{0}\" -s {1}x{2} {3} {4} \"{5}\"", inputFilePath.FullName,
				                                     message.Settings.Width, message.Settings.Height, position, AllFixedOptions,
				                                     outPutFilePath.FullName);
				var commandOutput = string.Empty;
				var ffmpegHelper = new FfMpegCommandLineExecutor();
				
				thumbnailCreationSuccessful = ExecuteFfMpegCommandLineExecutor(message, ffmpegHelper, message.WorkingDirectoryPath, commandPath, commandArguments, out commandOutput);
				
				output += videoInfoOutput + Environment.NewLine + commandOutput;
			}

			var executedMcAfeeWorkflowMessage = new ExecutedVideoThumbnailerWorkflowMessage()
			{
				CorrelationId = message.CorrelationId,
				ThumbnailCreationSuccessful = thumbnailCreationSuccessful,
				OutPutFilePath = outPutFilePath.FullName,
				Output = output
			};

			var bus = BusDriver.Instance.GetBus(VideoThumbnailerService.BusName);
			bus.Publish(executedMcAfeeWorkflowMessage);
		}
	}
}
