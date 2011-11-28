﻿using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Response
{
	public interface IMovedProcessedFileIntoOutputDirectoryMessage : ICommandIdentifier
	{
		Guid CorrelationId { get;  set; }
		string OutputFilePath { get; set; }
	}
}