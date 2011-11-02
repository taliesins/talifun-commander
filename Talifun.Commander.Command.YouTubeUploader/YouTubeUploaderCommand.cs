using System;
using System.IO;
using System.Threading;
using Google.GData.Client;
using Google.GData.Client.ResumableUpload;
using Google.YouTube;
using Talifun.Commander.Command.YouTubeUploader.Properties;

namespace Talifun.Commander.Command.YouTubeUploader
{
	public class YouTubeUploaderCommand : ICommand<YouTubeUploaderSettings>
	{
		public bool Run(YouTubeUploaderSettings settings, System.Configuration.AppSettingsSection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
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
								MediaSource = new MediaFileSource(outPutFilePath.FullName, contentType)
			            	};
			video.YouTubeEntry.Links.Add(link);

			var resumableUploader = new ResumableUploader(settings.Upload.ChunkSize);
			resumableUploader.AsyncOperationCompleted += OnResumableUploaderAsyncOperationCompleted;
			resumableUploader.AsyncOperationProgress += OnResumableUploaderAsyncOperationProgress;
			var uploadCompletedEventArgs = new UploadCompletedEventArgs
			                               	{
			                               		AutoResetEvent = new AutoResetEvent(false)
			                               	};

			resumableUploader.InsertAsync(youTubeAuthenticator, video.YouTubeEntry, uploadCompletedEventArgs);
			uploadCompletedEventArgs.AutoResetEvent.WaitOne();

			output = uploadCompletedEventArgs.Output;
			return uploadCompletedEventArgs.Result;
		}

		private void OnResumableUploaderAsyncOperationProgress(object sender, AsyncOperationProgressEventArgs e)
		{
			var uploadCompletedEventArgs = (UploadCompletedEventArgs)e.UserState;
			uploadCompletedEventArgs.Output += string.Format("Upload Progress: {0} ({1}/{2} - {3}%)", DateTime.Now, e.Position, e.CompleteSize, e.ProgressPercentage) + Environment.NewLine;
		}

		private void OnResumableUploaderAsyncOperationCompleted(object sender, AsyncOperationCompletedEventArgs e)
		{
			var uploadCompletedEventArgs = (UploadCompletedEventArgs)e.UserState;
			uploadCompletedEventArgs.Result = e.Cancelled || e.Error != null;
			if (e.Error != null)
			{
				uploadCompletedEventArgs.Output += e.Error.ToString();
			}
			uploadCompletedEventArgs.AutoResetEvent.Set();
		}
	}
}