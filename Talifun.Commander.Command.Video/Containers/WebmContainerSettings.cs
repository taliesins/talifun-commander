using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.VideoFormats;

namespace Talifun.Commander.Command.Video.Containers
{
	public class WebmContainerSettings : IContainerSettings
	{
		public WebmContainerSettings(IAudioSettings audio, IVideoSettings video)
		{
			FileNameExtension = "webm";
			Audio = audio;
			Video = video;
		}

		public string FileNameExtension { get; set; }
		public IAudioSettings Audio { get; set; }
		public IVideoSettings Video { get; set; }
	}
}
