using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Video.Command.Containers;

namespace Talifun.Commander.Command.Video.Command.Response
{
	public class RetrievedMetaDataMessage : CorrelatedMessageBase<RetrievedMetaDataMessage>
	{
		public ContainerMetaData MetaData { get; set; }
	}
}
