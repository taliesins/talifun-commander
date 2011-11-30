using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.VideoThumbNailer.Command.Response
{
	public interface IExecutedVideoThumbnailerWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		bool ThumbnailCreationSuccessful { get; set; }
		string OutPutFilePath { get; set; }
		string Output { get; set; }
	}
}
