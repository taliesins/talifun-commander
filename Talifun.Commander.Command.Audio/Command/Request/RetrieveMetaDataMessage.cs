using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio.Command.Request
{
	public class RetrieveMetaDataMessage : CorrelatedMessageBase<RetrieveMetaDataMessage>
	{
		public string InputFilePath { get; set; }
	}
}
