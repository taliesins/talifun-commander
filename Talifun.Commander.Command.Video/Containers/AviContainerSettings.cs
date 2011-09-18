using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.VideoFormats;

namespace Talifun.Commander.Command.Video.Containers
{
	public class AviContainerSettings: IContainerSettings
	{
		public AviContainerSettings(IAudioSettings audio, IVideoSettings video)
		{
			FileNameExtension = "avi";
			Audio = audio;
			Video = video;
		}

		public string FileNameExtension { get; private set; }
		public IAudioSettings Audio { get; private set; }
		public IVideoSettings Video { get; private set; }
	}
}
