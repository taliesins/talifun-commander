using System.Diagnostics;
using Talifun.Commander.Command.AntiVirus.CommandTester.Request;
using Talifun.Commander.Command.Esb.Request;

namespace Talifun.Commander.Command.AntiVirus.CommandTester
{
	public class AntiVirusTestConfigurationRequestMessageHandler : ICommandTestConfigurationRequestMessageHandler<AntiVirusTestConfigurationRequestMessage>
	{
		public void Consume(AntiVirusTestConfigurationRequestMessage message)
		{
			Trace.TraceInformation(message.GetType().Name);
		}
	}
}