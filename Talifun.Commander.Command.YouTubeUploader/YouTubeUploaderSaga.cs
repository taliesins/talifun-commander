﻿using System.IO;
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
				//var thumbnailerCommand = new ThumbnailerCommand();
				//youTubeUploadSuccessful = thumbnailerCommand.Run(thumbnailerSettings, properties.AppSettings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);

				if (!youTubeUploadSuccessful)
				{
					HandleError(output, properties, youTubeUploaderSetting.GetErrorProcessingPathOrDefault(), uniqueProcessingNumber);
				}
			}
			finally
			{
				Cleanup(workingDirectoryPath);
			}
		}
	}
}
