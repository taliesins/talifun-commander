using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.PicasaUploader.Command.Settings;

namespace Talifun.Commander.Command.PicasaUploader.Command.Response
{
	public class RetrievedMetaDataMessage : CorrelatedMessageBase<RetrievedMetaDataMessage>
	{
		public PicasaMetaData MetaData { get; set; }
	}
}
