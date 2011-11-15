using System.Runtime.Serialization;

namespace Talifun.Commander.Command.Image.Configuration
{
	public partial class ImageConversionElement
	{
		public ImageConversionElement(SerializationInfo info, StreamingContext context)
			: this()
		{
			SetObjectData(info, context);
		}
	}
}
