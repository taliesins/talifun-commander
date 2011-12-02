using System.Collections.Generic;

namespace Talifun.Commander.Command.Audio.Command.AudioFormats
{
	public interface IAudioMetaData : IDictionary<string, string>
	{
		string GetFfMpegCommandLineArgument();
	}
}
