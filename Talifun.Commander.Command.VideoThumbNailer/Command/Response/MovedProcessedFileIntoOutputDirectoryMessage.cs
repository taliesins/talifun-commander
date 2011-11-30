using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins.Response;

namespace Talifun.Commander.Command.VideoThumbNailer.Command.Response
{
	public class MovedProcessedFileIntoOutputDirectoryMessage : CorrelatedMessageBase<MovedProcessedFileIntoOutputDirectoryMessage>, IMovedProcessedFileIntoOutputDirectoryMessage
	{
		public string OutputFilePath { get; set; }
	}
}
