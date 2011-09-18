using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.VideoFormats;

namespace Talifun.Commander.Command.Video.Containers
{
	public class Mp4ContainerSettings : IContainerSettings
	{
		public Mp4ContainerSettings(IAudioSettings audio, IVideoSettings video)
		{
			FileNameExtension = "mp4";
			Audio = audio;
			Video = video;
		}

		public string FileNameExtension { get; private set; }
		public IAudioSettings Audio { get; private set; }
		public IVideoSettings Video { get; private set; }
	}
}
