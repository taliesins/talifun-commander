using System;
using System.Collections.Generic;

namespace Talifun.Commander.Command.Audio.Command.AudioFormats
{
	public class AudioMetaData : Dictionary<string, string>
	{
		public AudioMetaData()
			: base(StringComparer.OrdinalIgnoreCase)
		{}
	}
}
