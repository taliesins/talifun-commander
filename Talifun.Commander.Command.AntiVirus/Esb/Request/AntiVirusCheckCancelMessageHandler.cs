using System.Diagnostics;
using Talifun.Commander.Command.Esb.Request;

namespace Talifun.Commander.Command.AntiVirus.Esb.Request
{
	public class AntiVirusCheckCancelMessageHandler : ICommandCancelMessageHandler<AntiVirusCheckCancelMessage>
	{
		public void Consume(AntiVirusCheckCancelMessage message)
		{
			Trace.TraceInformation(message.GetType().Name);
		}
	}
}
