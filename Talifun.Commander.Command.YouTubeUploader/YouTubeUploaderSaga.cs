using System;
using System.IO;
using Talifun.Commander.Command.FileMatcher;
using Talifun.Commander.Command.YouTubeUploader.Configuration;

namespace Talifun.Commander.Command.YouTubeUploader
{
	public class YouTubeUploaderSaga : CommandSagaBase
	{
		public override ISettingConfiguration Settings
		{
			get
			{
				return YouTubeUploaderConfiguration.Instance;
			}
		}

		private YouTubeUploaderSettings GetYouTubeUploaderSettings(YouTubeUploaderElement youTubeUploader)
		{
			return new YouTubeUploaderSettings()
			{
				Authentication = new AuthenticationSettings()
				{
					GooglePassword = youTubeUploader.GooglePassword,
					GoogleUsername = youTubeUploader.GoogleUsername,
					YouTubeUsername = youTubeUploader.YouTubeUsername
				},
				Upload = new UploadSettings()
				         	{
				         		ChunkSize = 25
				         	}
			};
		}

		public override void Run(ICommandSagaProperties properties)
		{
			var youTubeUploaderSetting = GetSettings<YouTubeUploaderElementCollection, YouTubeUploaderElement>(properties);
			var uniqueProcessingNumber = Guid.NewGuid().ToString();
			var inputFilePath = new FileInfo(properties.InputFilePath);
			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(Settings.ConversionType, youTubeUploaderSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

			try
			{
				workingDirectoryPath.Create();

				var output = string.Empty;
				
				var youTubeUploadSuccessful = false;

				var youTubeUploaderSettings = GetYouTubeUploaderSettings(youTubeUploaderSetting);
				var youTubeUploaderCommand = new YouTubeUploaderCommand();
				youTubeUploadSuccessful = youTubeUploaderCommand.Run(youTubeUploaderSettings, properties.AppSettings, inputFilePath, workingDirectoryPath, out inputFilePath, out output);

				if (youTubeUploadSuccessful)
				{
					inputFilePath.MoveCompletedFileToOutputFolder(youTubeUploaderSetting.FileNameFormat, youTubeUploaderSetting.GetOutPutPathOrDefault());
				}
				else
				{
					HandleError(properties, uniqueProcessingNumber, inputFilePath, output, youTubeUploaderSetting.GetErrorProcessingPathOrDefault());
				}
			}
			finally
			{
				workingDirectoryPath.Cleanup();
			}
		}
	}
}
