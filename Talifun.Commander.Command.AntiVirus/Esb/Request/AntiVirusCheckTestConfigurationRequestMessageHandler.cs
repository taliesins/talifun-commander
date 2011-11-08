using System.Diagnostics;
using Talifun.Commander.Command.Esb.Request;

namespace Talifun.Commander.Command.AntiVirus.Esb.Request
{
	public class AntiVirusCheckTestConfigurationRequestMessageHandler : ICommandTestConfigurationRequestMessageHandler<AntiVirusCheckTestConfigurationRequestMessage>
	{
		public void Consume(AntiVirusCheckTestConfigurationRequestMessage message)
		{
			Trace.TraceInformation(message.GetType().Name);
		}
	}
}