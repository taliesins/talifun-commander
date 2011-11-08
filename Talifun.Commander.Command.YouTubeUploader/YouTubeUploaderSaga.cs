using System.IO;
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
			var uniqueProcessingNumber = UniqueIdentifier();
			var workingDirectoryPath = GetWorkingDirectoryPath(properties, youTubeUploaderSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

			try
			{
				workingDirectoryPath.Create();

				var output = string.Empty;
				FileInfo workingFilePath = null;

				var youTubeUploadSuccessful = false;

				var youTubeUploaderSettings = GetYouTubeUploaderSettings(youTubeUploaderSetting);
				var youTubeUploaderCommand = new YouTubeUploaderCommand();
				youTubeUploadSuccessful = youTubeUploaderCommand.Run(youTubeUploaderSettings, properties.AppSettings, new FileInfo(properties.InputFilePath), workingDirectoryPath, out workingFilePath, out output);

				if (youTubeUploadSuccessful)
				{
					MoveCompletedFileToOutputFolder(workingFilePath, youTubeUploaderSetting.FileNameFormat, youTubeUploaderSetting.GetOutPutPathOrDefault());
				}
				else
				{
					HandleError(properties, uniqueProcessingNumber, workingFilePath, output, youTubeUploaderSetting.GetErrorProcessingPathOrDefault());
				}
			}
			finally
			{
				Cleanup(workingDirectoryPath);
			}
		}
	}
}
