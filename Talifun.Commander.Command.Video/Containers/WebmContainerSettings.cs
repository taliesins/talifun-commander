using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.VideoFormats;
using Talifun.Commander.Command.Video.Watermark;

namespace Talifun.Commander.Command.Video.Containers
{
	public class WebmContainerSettings : IContainerSettings
	{
		public WebmContainerSettings(IAudioSettings audio, IVideoSettings video, IWatermarkSettings watermark)
		{
			FileNameExtension = "webm";
			Audio = audio;
			Audio.CodecName = "libvorbis"; //This is the only supported audio codec for the webm container
			Video = video;
			Watermark = watermark;
		}

		public string FileNameExtension { get; set; }
		public string IntroPath { get; set; }
		public string OuttroPath { get; set; }
		public IAudioSettings Audio { get; set; }
		public IVideoSettings Video { get; set; }
		public IWatermarkSettings Watermark { get; private set; }
	}
}
