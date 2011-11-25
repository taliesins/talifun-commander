using System;
using System.IO;
using System.Threading;
using Google.GData.Client;
using Google.GData.Client.ResumableUpload;
using Google.GData.Extensions.MediaRss;
using Google.YouTube;
using NLog;
using Talifun.Commander.Command.YouTubeUploader.Properties;

namespace Talifun.Commander.Command.YouTubeUploader
{
	public class YouTubeUploaderCommand : ICommand<IYouTubeUploaderSettings>
	{
		private Logger _logger = LogManager.GetCurrentClassLogger();

		public bool Run(IYouTubeUploaderSettings settings, System.Configuration.AppSettingsSection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
		{
			outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, inputFilePath.Name));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			inputFilePath.CopyTo(outPutFilePath.FullName);

			var credentials = new GDataCredentials(settings.Authentication.GoogleUsername, settings.Authentication.GooglePassword);
			var youTubeAuthenticator = new ClientLoginAuthenticator(settings.Authentication.ApplicationName, ServiceNames.YouTube, credentials)
			                           	{
			                           		DeveloperKey = settings.Authentication.DeveloperKey
			                           	};

			var contentType = MediaFileSource.GetContentTypeForFileName(outPutFilePath.FullName);

			var link = new AtomLink(string.Format(Resource.UploadLink, settings.Authentication.YouTubeUsername))
			{
				Rel = ResumableUploader.CreateMediaRelation
			};

			var video = new Video
			            	{
								Title = "Test Video",
								Description = "Test Description",
								Keywords = "TalifunCommander",
								Private = true,
								MediaSource = new MediaFileSource(outPutFilePath.FullName, contentType)
			            	};

			video.Tags.Add(new MediaCategory("TalifunCommander"));
			video.YouTubeEntry.Links.Add(link);

			var resumableUploader = new ResumableUploader(settings.Upload.ChunkSize);
			resumableUploader.AsyncOperationCompleted += new AsyncOperationCompletedEventHandler(OnResumableUploaderAsyncOperationCompleted);
			resumableUploader.AsyncOperationProgress += new AsyncOperationProgressEventHandler(OnResumableUploaderAsyncOperationProgress);
			var uploadCompletedEventArgs = new UploadCompletedEventArgs
			                               	{
			                               		Output = string.Empty,
												Result = false
			                               	};

			resumableUploader.InsertAsync(youTubeAuthenticator, video.YouTubeEntry, uploadCompletedEventArgs);
			

			Thread.Yield();
			
			output = uploadCompletedEventArgs.Output;

			

			return uploadCompletedEventArgs.Result;
		}

		private void OnResumableUploaderAsyncOperationProgress(object sender, AsyncOperationProgressEventArgs e)
		{
			var message = string.Format("Upload Progress: {0} ({1}/{2} - {3}%) {4} {5}", DateTime.Now, e.Position, e.CompleteSize,
			                            e.ProgressPercentage, e.HttpVerb, e.Uri);

			_logger.Info(message);

			var uploadCompletedEventArgs = e.UserState as UploadCompletedEventArgs;
			if (uploadCompletedEventArgs != null)
			{
				uploadCompletedEventArgs.Output += message + Environment.NewLine;
			}
		}

		private void OnResumableUploaderAsyncOperationCompleted(object sender, AsyncOperationCompletedEventArgs e)
		{
			var uploadCompletedEventArgs = (UploadCompletedEventArgs)e.UserState;
			uploadCompletedEventArgs.Result =  e.Error != null || e.Cancelled;
			if (e.Error != null)
			{
				uploadCompletedEventArgs.Output += e.Error.ToString();
			}
		}
	}
}