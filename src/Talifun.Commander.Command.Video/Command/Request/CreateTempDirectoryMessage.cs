using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins.Request;

namespace Talifun.Commander.Command.Video.Command.Request
{
	public class CreateTempDirectoryMessage : CorrelatedMessageBase<CreateTempDirectoryMessage>, ICreateTempDirectoryMessage
	{
		public string Prefix { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
