using System.Runtime.Serialization;

namespace Talifun.Commander.Command.Audio.Configuration
{
	public partial class AudioConversionElement
	{
		public AudioConversionElement(SerializationInfo info, StreamingContext context)
			: this()
		{
			SetObjectData(info, context);
		}
	}
}
