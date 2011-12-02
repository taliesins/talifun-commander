using System.Collections.Generic;

namespace Talifun.Commander.Command.Video.Command.Containers
{
	public interface IContainerMetaData : IDictionary<string, string>
	{
		string GetFfMpegCommandLineArgument();
	}
}
