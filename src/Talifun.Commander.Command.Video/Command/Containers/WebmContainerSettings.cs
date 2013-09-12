using System.Collections.Generic;
using Talifun.Commander.Command.Video.Command.AudioFormats;
using Talifun.Commander.Command.Video.Command.VideoFormats;
using Talifun.Commander.Command.Video.Command.Watermark;

namespace Talifun.Commander.Command.Video.Command.Containers
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

			AllowedMetaData = new List<string>
			               	{
			               		"Title",
								"Description",
								"Langauge",
			               	};
		}

		public string FileNameExtension { get; set; }
		public string IntroPath { get; set; }
		public string OuttroPath { get; set; }
		public IAudioSettings Audio { get; set; }
		public IVideoSettings Video { get; set; }
		public IWatermarkSettings Watermark { get; private set; }
		public List<string> AllowedMetaData { get; set; }
		public ContainerMetaData MetaData { get; set; }
	}
}
