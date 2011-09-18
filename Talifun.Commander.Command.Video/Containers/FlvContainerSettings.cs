using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.VideoFormats;

namespace Talifun.Commander.Command.Video.Containers
{
	public class FlvContainerSettings : IContainerSettings
	{
		public FlvContainerSettings(IAudioSettings audio, IVideoSettings video)
		{
			FileNameExtension = "flv";
			Audio = audio;
			Video = video;
		}

		public string FileNameExtension { get; private set; }
		public IAudioSettings Audio { get; private set; }
		public IVideoSettings Video { get; private set; }
	}
}
