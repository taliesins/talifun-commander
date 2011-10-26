using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.VideoFormats;

namespace Talifun.Commander.Command.Video.Containers
{
	public interface IContainerSettings
	{
		string FileNameExtension { get; }
		IAudioSettings Audio { get; }
		IVideoSettings Video { get; }
		IWatermarkSettings Watermark { get; }
	}
}