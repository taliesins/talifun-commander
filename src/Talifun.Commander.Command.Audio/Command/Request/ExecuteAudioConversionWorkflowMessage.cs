using System.Collections.Generic;
using Talifun.Commander.Command.Audio.Command.AudioFormats;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio.Command.Request
{
	public class ExecuteAudioConversionWorkflowMessage : CorrelatedMessageBase<ExecuteAudioConversionWorkflowMessage>, IExecuteAudioConversionWorkflowMessage
	{
		public IAudioSettings Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
