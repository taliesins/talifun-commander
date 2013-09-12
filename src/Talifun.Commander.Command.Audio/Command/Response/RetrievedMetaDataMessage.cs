using Talifun.Commander.Command.Audio.Command.AudioFormats;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio.Command.Response
{
	public class RetrievedMetaDataMessage : CorrelatedMessageBase<RetrievedMetaDataMessage>
	{
		public AudioMetaData MetaData { get; set; }
	}
}
