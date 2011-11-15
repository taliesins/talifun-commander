using System.Runtime.Serialization;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
	public partial class AntiVirusElement
	{
		public AntiVirusElement(SerializationInfo info, StreamingContext context)
			: this()
		{
			SetObjectData(info, context);
		}
	}
}
