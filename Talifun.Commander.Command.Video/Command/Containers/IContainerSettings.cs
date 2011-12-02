using System.Collections.Generic;
using Talifun.Commander.Command.Video.Command.AudioFormats;
using Talifun.Commander.Command.Video.Command.VideoFormats;
using Talifun.Commander.Command.Video.Command.Watermark;

namespace Talifun.Commander.Command.Video.Command.Containers
{
	public interface IContainerSettings
	{
		string FileNameExtension { get; }
		string IntroPath { get; }
		string OuttroPath { get; }
		IAudioSettings Audio { get; }
		IVideoSettings Video { get; }
		IWatermarkSettings Watermark { get; }
		List<string> AllowedMetaData { get; set; }
		ContainerMetaData MetaData { get; set; }
	}
}