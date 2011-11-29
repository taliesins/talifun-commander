﻿using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins.Request;

namespace Talifun.Commander.Command.Video.Command.Request
{
	public class MoveProcessedFileIntoErrorDirectoryMessage : CorrelatedMessageBase<MoveProcessedFileIntoErrorDirectoryMessage>, IMoveProcessedFileIntoErrorDirectoryMessage
	{
		public string ErrorDirectoryPath { get; set; }
		public string OutputFilePath { get; set; }
	}
}
