using System.Runtime.Serialization;

namespace Talifun.Commander.Command.Configuration
{
	public partial class ProjectElement
	{
		public ProjectElement(SerializationInfo info, StreamingContext context)
			: this()
		{
			SetObjectData(info, context);
		}
	}
}
