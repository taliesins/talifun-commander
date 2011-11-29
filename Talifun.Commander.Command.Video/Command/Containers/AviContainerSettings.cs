using Talifun.Commander.Command.Video.Command.AudioFormats;
using Talifun.Commander.Command.Video.Command.VideoFormats;
using Talifun.Commander.Command.Video.Command.Watermark;

namespace Talifun.Commander.Command.Video.Command.Containers
{
	public class AviContainerSettings: IContainerSettings
	{
		public AviContainerSettings(IAudioSettings audio, IVideoSettings video, IWatermarkSettings watermark)
		{
			FileNameExtension = "avi";
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
