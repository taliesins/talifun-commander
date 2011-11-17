using System.Diagnostics;
using Talifun.Commander.Command.AntiVirus.CommandTester.Request;
using Talifun.Commander.Command.Esb.Request;

namespace Talifun.Commander.Command.AntiVirus.CommandTester
{
	public class AntiVirusConfigurationTestRequestMessageHandler : ICommandConfigurationTestRequestMessageHandler<AntiVirusConfigurationTestRequestMessage>
	{
		public void Consume(AntiVirusConfigurationTestRequestMessage message)
		{
			Trace.TraceInformation(message.GetType().Name);
		}
	}
}