using System.IO;
using MassTransit;
using Talifun.Commander.Command.FlickrUploader.Command.Request;

namespace Talifun.Commander.Command.FlickrUploader.Command
{
	public class ExecuteFlickrUploaderWorkflowMessageHandler : ExecuteFlickrUploaderWorkflowMessageHandlerBase, Consumes<ExecuteFlickrUploaderWorkflowMessage>.All
	{
		public void Consume(ExecuteFlickrUploaderWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);

			CheckAuthenticationToken(message.Settings.Authentication);
			var authenticator = new FlickrNet.Uploader.FlickrAuthenticator(message.Settings.Authentication.FlickrApiKey, message.Settings.Authentication.FlickrApiSecret, message.Settings.Authentication.FlickrAuthToken);

			ExecuteUpload(message, authenticator, inputFilePath);
		}
	}
}
