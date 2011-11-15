using System.Runtime.Serialization;

namespace Talifun.Commander.Command.YouTubeUploader.Configuration
{
	public partial class YouTubeUploaderElement
	{
		public YouTubeUploaderElement(SerializationInfo info, StreamingContext context)
			: this()
		{
			SetObjectData(info, context);
		}
	}
}
