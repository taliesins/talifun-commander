using System.IO;
using Google.GData.Client;
using Google.GData.Client.ResumableUpload;
using Google.GData.Extensions.MediaRss;
using Google.GData.Photos;
using MassTransit;
using Talifun.Commander.Command.PicasaUploader.Command.Request;
using Talifun.Commander.Command.PicasaUploader.Properties;

namespace Talifun.Commander.Command.PicasaUploader.Command
{
	public class ExecutePicasaUploaderWorkflowMessageHandler : ExecutePicasaUploaderWorkflowMessageHandlerBase, Consumes<ExecutePicasaUploaderWorkflowMessage>.All
	{
		public void Consume(ExecutePicasaUploaderWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);

			var credentials = new GDataCredentials(message.Settings.Authentication.GoogleUsername, message.Settings.Authentication.GooglePassword);
			var picasaAuthenticator = new ClientLoginAuthenticator(message.Settings.Authentication.ApplicationName, PicasaService.GPicasaService, credentials)
			{
			};

			var contentType = MediaFileSource.GetContentTypeForFileName(inputFilePath.FullName);

			var albumId = message.Settings.MetaData.AlbumId != "default" && !string.IsNullOrEmpty(message.Settings.MetaData.AlbumId)
				? message.Settings.MetaData.AlbumId
				: message.Settings.Authentication.AlbumId != "default" && !string.IsNullOrEmpty(message.Settings.Authentication.AlbumId) 
				? message.Settings.Authentication.AlbumId 
				: "default";

			var link = new AtomLink(string.Format(Resource.UploadLink, albumId))
			{
				Rel = ResumableUploader.CreateMediaRelation
			};
			
			var photoEntry = new PhotoEntry();
			
			if (message.Settings.MetaData.Title != null)
			{
				photoEntry.Title.Text = message.Settings.MetaData.Title;

				if (photoEntry.Media == null)
				{
					photoEntry.Media = new MediaGroup();
				}
				photoEntry.Media.Title = new MediaTitle(message.Settings.MetaData.Title);
			}

			if (message.Settings.MetaData.Description != null)
			{
				photoEntry.Summary.Text = message.Settings.MetaData.Description;

				if (photoEntry.Media == null)
				{
					photoEntry.Media = new MediaGroup();
				}
				photoEntry.Media.Description = new MediaDescription(message.Settings.MetaData.Description);
			}

			if (message.Settings.MetaData.Keywords != null)
			{
				if (photoEntry.Media == null)
				{
					photoEntry.Media = new MediaGroup();
				}
				photoEntry.Media.Keywords = new MediaKeywords(message.Settings.MetaData.Keywords);
			}

			if (message.Settings.MetaData.Credit != null)
			{
				if (photoEntry.Media == null)
				{
					photoEntry.Media = new MediaGroup();
				}
				photoEntry.Media.Credit = new MediaCredit(message.Settings.MetaData.Credit);
			}

			if (message.Settings.MetaData.Published.HasValue)
			{
				photoEntry.Published = message.Settings.MetaData.Published.Value;	
			}

			if (message.Settings.MetaData.Updated.HasValue)
			{
				photoEntry.Updated = message.Settings.MetaData.Updated.Value;	
			}

			photoEntry.MediaSource = new MediaFileSource(inputFilePath.FullName, contentType);
			photoEntry.Links.Add(link);
			
			ExecuteUpload(message, picasaAuthenticator, photoEntry);
		}
	}
}
