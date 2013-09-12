using System.IO;
using Talifun.Commander.Command.FlickrUploader.Command.Request;

namespace Talifun.Commander.Command.FlickrUploader.Command
{
	public class AsyncUploadSettings
	{
		public Stream InputStream { get; set; }
		public IExecuteFlickrUploaderWorkflowMessage Message { get; set; }
	}
}
