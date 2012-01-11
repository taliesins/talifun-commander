using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.VideoThumbNailer.Command.Response
{
	public interface IExecutedVideoThumbnailerWorkflowMessage : ICommandIdentifier
	{
		bool ThumbnailCreationSuccessful { get; set; }
		string OutPutFilePath { get; set; }
		string Output { get; set; }
	}
}
