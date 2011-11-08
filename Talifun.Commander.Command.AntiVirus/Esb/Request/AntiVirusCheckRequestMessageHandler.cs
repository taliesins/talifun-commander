using System.Diagnostics;
using Talifun.Commander.Command.Esb.Request;

namespace Talifun.Commander.Command.AntiVirus.Esb.Request
{
	public class AntiVirusCheckRequestMessageHandler : ICommandRequestMessageHandler<AntiVirusCheckRequestMessage>
	{
		public void Consume(AntiVirusCheckRequestMessage message)
		{
			Trace.TraceInformation(message.GetType().Name);
		}
	}
}
