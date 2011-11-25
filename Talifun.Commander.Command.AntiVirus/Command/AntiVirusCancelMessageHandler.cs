using System.Diagnostics;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.Esb.Request;

namespace Talifun.Commander.Command.AntiVirus.Command
{
	public class AntiVirusCancelMessageHandler : ICancelMessageHandler<AntiVirusCancelMessage>
	{
		public void Consume(AntiVirusCancelMessage message)
		{
			Trace.TraceInformation(message.GetType().Name);
		}
	}
}
