using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.ImageSettings;

namespace Talifun.Commander.Command.Image.Command.Response
{
	public class RetrievedMetaDataMessage : CorrelatedMessageBase<RetrievedMetaDataMessage>
	{
		public ImageMetaData MetaData { get; set; }
	}
}
