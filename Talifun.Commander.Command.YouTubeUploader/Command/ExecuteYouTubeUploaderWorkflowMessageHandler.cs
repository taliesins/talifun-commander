using System.IO;
using Google.GData.Client;
using Google.GData.Client.ResumableUpload;
using Google.GData.Extensions.MediaRss;
using Google.YouTube;
using MassTransit;
using Talifun.Commander.Command.YouTubeUploader.Command.Request;
using Talifun.Commander.Command.YouTubeUploader.Properties;

namespace Talifun.Commander.Command.YouTubeUploader.Command
{
	public class ExecuteYouTubeUploaderWorkflowMessageHandler : ExecuteYouTubeUploaderWorkflowMessageHandlerBase, Consumes<ExecuteYouTubeUploaderWorkflowMessage>.All
	{
		public void Consume(ExecuteYouTubeUploaderWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);

			var credentials = new GDataCredentials(message.Settings.Authentication.GoogleUsername, message.Settings.Authentication.GooglePassword);
			var youTubeAuthenticator = new ClientLoginAuthenticator(message.Settings.Authentication.ApplicationName, ServiceNames.YouTube, credentials)
			{
				DeveloperKey = message.Settings.Authentication.DeveloperKey
			};

			var contentType = MediaFileSource.GetContentTypeForFileName(inputFilePath.FullName);

			var link = new AtomLink(string.Format(Resource.UploadLink, message.Settings.Authentication.YouTubeUsername))
			{
				Rel = ResumableUploader.CreateMediaRelation
			};

			var video = new Video
			{
				Title = message.Settings.MetaData.Title,
				Description = message.Settings.MetaData.Description,
				Keywords = message.Settings.MetaData.Keywords,
				Private = message.Settings.MetaData.Private,
				MediaSource = new MediaFileSource(inputFilePath.FullName, contentType)
			};

			foreach (var category in message.Settings.MetaData.Categories)
			{
				video.Tags.Add(new MediaCategory(category));
			}

			video.YouTubeEntry.Links.Add(link);

			ExecuteUpload(message, youTubeAuthenticator, video.YouTubeEntry);
		}
	}
}
