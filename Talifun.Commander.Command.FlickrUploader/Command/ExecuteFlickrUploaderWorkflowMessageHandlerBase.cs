using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FlickrNet;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FlickrUploader.Command.Request;
using Talifun.Commander.Command.FlickrUploader.Command.Settings;

namespace Talifun.Commander.Command.FlickrUploader.Command
{
	public abstract class ExecuteFlickrUploaderWorkflowMessageHandlerBase
	{
		protected void ExecuteUpload(IExecuteFlickrUploaderWorkflowMessage message, Flickr flickr, FileInfo inputFilePath)
		{
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var task = Task.Factory.StartNew(() => { });
			task.ContinueWith((t) =>
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					FlickrUploaderService.Uploaders.Add(message, new CancellableTask
					{
						Task = task,
						CancellationTokenSource = cancellationTokenSource
					});

                    var asyncUploadSettings = new AsyncUploadSettings
                    {
                        Message = message,
                        InputStream = inputFilePath.OpenRead()
                    };

				    var uploader = new Uploader(asyncUploadSettings);

                    flickr.OnUploadProgress += uploader.OnUploadProgress;

                    flickr.UploadPictureAsync(asyncUploadSettings.InputStream, inputFilePath.Name,
                                message.Settings.MetaData.Title,
                                message.Settings.MetaData.Description,
                                message.Settings.MetaData.Keywords,
                                message.Settings.MetaData.IsPublic,
                                message.Settings.MetaData.IsFamily,
                                message.Settings.MetaData.IsFriend,
                                message.Settings.MetaData.ContentType,
                                message.Settings.MetaData.SafetyLevel,
                                message.Settings.MetaData.HiddenFromSearch, 
                                result =>
                                    {
                                        FlickrUploaderService.Uploaders.Remove(message);
                                        uploader.OnUploadCompleted(result);
                                    } 
                                );
				}
				cancellationToken.ThrowIfCancellationRequested();
			}
				, cancellationToken);
		}

	    protected Auth CheckAuthenticationToken(IAuthenticationSettings authenticationSettings)
		{
			if (string.IsNullOrEmpty(authenticationSettings.FlickrAuthToken))
			{
				throw new Exception("No Flickr AuthToken!");
			}

			Flickr.CacheTimeout = new TimeSpan(1, 0, 0, 0, 0);

			var flickrService = new Flickr(authenticationSettings.FlickrApiKey, authenticationSettings.FlickrApiSecret, authenticationSettings.FlickrAuthToken);
			var authenticationToken = flickrService.AuthCheckToken(authenticationSettings.FlickrAuthToken);

			return authenticationToken;
		}
	}
}
