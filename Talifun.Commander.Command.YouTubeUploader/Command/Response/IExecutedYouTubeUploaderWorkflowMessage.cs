using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.YouTubeUploader.Command.Response
{
	public interface IExecutedYouTubeUploaderWorkflowMessage : ICommandIdentifier
	{
		Exception Error { get; set; }
		bool Cancelled { get; set; }
		string VideoId { get; set; }
	}
}
