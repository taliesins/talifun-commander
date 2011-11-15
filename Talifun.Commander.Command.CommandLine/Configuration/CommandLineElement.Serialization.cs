using System.Runtime.Serialization;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
	public partial class CommandLineElement
	{
		public CommandLineElement(SerializationInfo info, StreamingContext context)
			: this()
		{
			SetObjectData(info, context);
		}
	}
}
