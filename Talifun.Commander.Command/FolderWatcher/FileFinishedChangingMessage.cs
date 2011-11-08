using System;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.FolderWatcher
{
	[Serializable]
	public class FileFinishedChangingMessage
	{
		public string FilePath { get; set; }
		public FolderElement Folder { get; set; }
	}
}
