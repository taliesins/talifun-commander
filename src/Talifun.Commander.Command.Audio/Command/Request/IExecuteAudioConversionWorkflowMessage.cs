using System.Collections.Generic;
using Talifun.Commander.Command.Audio.Command.AudioFormats;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio.Command.Request
{
	public interface IExecuteAudioConversionWorkflowMessage : ICommandIdentifier
	{
		IAudioSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
		string WorkingDirectoryPath { get; set; }
	}
}
