using System;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FolderWatcher.Messages
{
	[Serializable]
	public class FileFinishedChangingMessage : CorrelatedMessageBase<FileFinishedChangingMessage> 
	{
		public string FilePath { get; set; }
		public FolderElement Folder { get; set; }
	}
}
