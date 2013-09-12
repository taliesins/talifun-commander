using System.IO;
using Google.GData.Client;
using Google.GData.Client.ResumableUpload;
using Google.GData.Extensions.MediaRss;
using Google.GData.YouTube;
using Google.YouTube;
using MassTransit;
using Talifun.Commander.Command.YouTubeUploader.Command.Request;
using Talifun.Commander.Command.YouTubeUploader.Properties;
using MediaCredit = Google.GData.YouTube.MediaCredit;
using MediaGroup = Google.GData.YouTube.MediaGroup;

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

			//YouTube requires that a video has a title and belongs to a category
			var video = new Video
			            	{
								Title = message.Settings.MetaData.Title,
			            		Private = message.Settings.MetaData.Private
			            	};

			video.Tags.Add(new MediaCategory(message.Settings.MetaData.Category, YouTubeNameTable.CategorySchema));


			if (message.Settings.MetaData.Description != null)
			{
				video.Description = message.Settings.MetaData.Description;
			}

			if (message.Settings.MetaData.Keywords != null)
			{
				video.Keywords = message.Settings.MetaData.Keywords;
			}

			if (message.Settings.MetaData.Keywords != null)
			{
				video.Keywords = message.Settings.MetaData.Keywords;
			}

			if (message.Settings.MetaData.Updated.HasValue)
			{
				video.Updated = message.Settings.MetaData.Updated.Value;
			}

			if (message.Settings.MetaData.Credit != null)
			{
				if (video.Media == null)
				{
					video.Media = new MediaGroup();
				}

				video.Media.Credit = new MediaCredit()
				{
				    Value = message.Settings.MetaData.Credit
				};
			}

			foreach (var developerCategory in message.Settings.MetaData.DeveloperTags)
			{
				video.Tags.Add(new MediaCategory(developerCategory, YouTubeNameTable.DeveloperTagSchema));
			}

			video.MediaSource = new MediaFileSource(inputFilePath.FullName, contentType);
			video.YouTubeEntry.Links.Add(link);

			ExecuteUpload(message, youTubeAuthenticator, video.YouTubeEntry);
		}
	}
}
