using System.Runtime.Serialization;

namespace Talifun.Commander.Command.Configuration
{
	public partial class FolderElement
	{
		public FolderElement(SerializationInfo info, StreamingContext context)
			: this()
		{
			SetObjectData(info, context);
		}
	}
}
