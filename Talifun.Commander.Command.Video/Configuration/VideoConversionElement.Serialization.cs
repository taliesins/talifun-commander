using System.Runtime.Serialization;

namespace Talifun.Commander.Command.Video.Configuration
{
	public partial class VideoConversionElement
	{
		public VideoConversionElement(SerializationInfo info, StreamingContext context)
			: this()
		{
			SetObjectData(info, context);
		}
	}
}
