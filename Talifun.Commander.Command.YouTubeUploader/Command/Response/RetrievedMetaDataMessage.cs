using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.YouTubeUploader.Command.Settings;

namespace Talifun.Commander.Command.YouTubeUploader.Command.Response
{
	public class RetrievedMetaDataMessage : CorrelatedMessageBase<RetrievedMetaDataMessage>
	{
		public YouTubeMetaData MetaData { get; set; }
	}
}
