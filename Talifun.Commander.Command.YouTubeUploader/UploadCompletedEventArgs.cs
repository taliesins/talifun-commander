using System;
using System.Threading;

namespace Talifun.Commander.Command.YouTubeUploader
{
	public class UploadCompletedEventArgs
	{
		public bool Result { get; set; }
		public string Output { get; set; }
		public AutoResetEvent AutoResetEvent { get; set; }
	}
}
