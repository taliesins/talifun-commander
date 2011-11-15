using System.Runtime.Serialization;

namespace Talifun.Commander.Command.Configuration
{
	public partial class FileMatchElement
	{
		public FileMatchElement(SerializationInfo info, StreamingContext context)
			: this()
		{
			SetObjectData(info, context);
		}
	}
}
