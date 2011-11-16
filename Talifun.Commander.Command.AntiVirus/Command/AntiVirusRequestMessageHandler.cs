using System.Diagnostics;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.Esb.Request;

namespace Talifun.Commander.Command.AntiVirus.Command
{
	public class AntiVirusRequestMessageHandler : ICommandRequestMessageHandler<AntiVirusRequestMessage>
	{
		public void Consume(AntiVirusRequestMessage message)
		{
			Trace.TraceInformation(message.GetType().Name);
		}
	}
}
