using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.VideoFormats;
using Talifun.Commander.Command.Video.Watermark;

namespace Talifun.Commander.Command.Video.Containers
{
	public class Mp4ContainerSettings : IContainerSettings
	{
		public Mp4ContainerSettings(IAudioSettings audio, IVideoSettings video, IWatermarkSettings watermark)
		{
			FileNameExtension = "mp4";
			Audio = audio;
			Video = video;
			Watermark = watermark;
		}

		public string FileNameExtension { get; private set; }
		public string IntroPath { get; set; }
		public string OuttroPath { get; set; }
		public IAudioSettings Audio { get; private set; }
		public IVideoSettings Video { get; private set; }
		public IWatermarkSettings Watermark { get; private set; }
	}
}
