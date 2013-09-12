using System.Collections.Generic;
using Talifun.Commander.Command.Video.Command.AudioFormats;
using Talifun.Commander.Command.Video.Command.VideoFormats;
using Talifun.Commander.Command.Video.Command.Watermark;

namespace Talifun.Commander.Command.Video.Command.Containers
{
	public class Mp4ContainerSettings : IContainerSettings
	{
		public Mp4ContainerSettings(IAudioSettings audio, IVideoSettings video, IWatermarkSettings watermark)
		{
			FileNameExtension = "mp4";
			Audio = audio;
			Video = video;
			Watermark = watermark;
			AllowedMetaData = new List<string>
			               	{
			               		"Title",
								"Artist",
								"AlbumArtist",
			               		"Album",
			               		"Grouping",
			               		"Composer",
			               		"Year",
								"Track",
			               		"Comment",
								"Genre",
								"Copyright",
								"Description",
								"Synopsis",
								"Show",
								"EpisodeId",
								"Network",
								"Lyrics",
			               	};
		}

		public string FileNameExtension { get; private set; }
		public string IntroPath { get; set; }
		public string OuttroPath { get; set; }
		public IAudioSettings Audio { get; private set; }
		public IVideoSettings Video { get; private set; }
		public IWatermarkSettings Watermark { get; private set; }
		public List<string> AllowedMetaData { get; set; }
		public ContainerMetaData MetaData { get; set; }
	}
}
