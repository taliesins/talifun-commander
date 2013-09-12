using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FlickrUploader.Command.Settings;

namespace Talifun.Commander.Command.FlickrUploader.Command.Response
{
	public class RetrievedMetaDataMessage : CorrelatedMessageBase<RetrievedMetaDataMessage>
	{
		public FlickrMetaData MetaData { get; set; }
	}
}
