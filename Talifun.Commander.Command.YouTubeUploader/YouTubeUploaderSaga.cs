using System;
using System.IO;
using Magnum;
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

		private IYouTubeUploaderSettings GetCommandSettings(YouTubeUploaderElement youTubeUploader)
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

		private ICommand<IYouTubeUploaderSettings> GetCommand(IYouTubeUploaderSettings youTubeUploaderSettings)
		{
			return new YouTubeUploaderCommand();
		}

		public override void Run(ICommandSagaProperties properties)
		{
			var commandElement = properties.Project.GetElement<YouTubeUploaderElement>(properties.FileMatch, Settings.ElementCollectionSettingName);

			var uniqueProcessingNumber = CombGuid.Generate().ToString();
			var inputFilePath = new FileInfo(properties.InputFilePath);
			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(Settings.ConversionType, commandElement.GetWorkingPathOrDefault(), uniqueProcessingNumber);

			try
			{
				workingDirectoryPath.Create();

				var output = string.Empty;

				var commandSettings = GetCommandSettings(commandElement);
				var command = GetCommand(commandSettings);
				var youTubeUploadSuccessful = command.Run(commandSettings, properties.AppSettings, inputFilePath, workingDirectoryPath, out inputFilePath, out output);

				if (youTubeUploadSuccessful)
				{
					inputFilePath.MoveCompletedFileToOutputFolder(commandElement.FileNameFormat, commandElement.GetOutPutPathOrDefault());
				}
				else
				{
					HandleError(properties, uniqueProcessingNumber, inputFilePath, output, commandElement.GetErrorProcessingPathOrDefault());
				}
			}
			finally
			{
				workingDirectoryPath.Cleanup();
			}
		}
	}
}
