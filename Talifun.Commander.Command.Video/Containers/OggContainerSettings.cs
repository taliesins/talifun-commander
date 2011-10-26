using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.VideoFormats;

namespace Talifun.Commander.Command.Video.Containers
{
	public class OggContainerSettings : IContainerSettings
	{
		public OggContainerSettings(IAudioSettings audio, IVideoSettings video, IWatermarkSettings watermark)
		{
			FileNameExtension = "ogv";
			Audio = audio;
			Video = video;
			Watermark = watermark;
		}

		public string FileNameExtension { get; set; }
		public IAudioSettings Audio { get; set; }
		public IVideoSettings Video { get; set; }
		public IWatermarkSettings Watermark { get; private set; }
	}
}
