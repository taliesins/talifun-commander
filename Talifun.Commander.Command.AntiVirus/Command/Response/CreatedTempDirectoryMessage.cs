﻿using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins.Response;

namespace Talifun.Commander.Command.AntiVirus.Command.Response
{
	public class CreatedTempDirectoryMessage : CorrelatedMessageBase<CreatedTempDirectoryMessage>, ICreatedTempDirectoryMessage
	{
		public string WorkingDirectoryPath { get; set; }
	}
}
