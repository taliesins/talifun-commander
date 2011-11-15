using System.Runtime.Serialization;

namespace Talifun.Commander.Command.VideoThumbNailer.Configuration
{
	public partial class VideoThumbnailerElement
	{
		public VideoThumbnailerElement(SerializationInfo info, StreamingContext context)
			: this()
		{
			SetObjectData(info, context);
		}
	}
}
