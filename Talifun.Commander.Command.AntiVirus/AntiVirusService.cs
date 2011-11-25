using Talifun.Commander.Command.AntiVirus.Command;
using Talifun.Commander.Command.AntiVirus.CommandTester;
using Talifun.Commander.Command.AntiVirus.Configuration;

namespace Talifun.Commander.Command.AntiVirus
{
	public class AntiVirusService : CommandServiceBase<AntiVirusSaga, AntiVirusTesterSaga>
	{
		static AntiVirusService()
		{
			Settings = AntiVirusConfiguration.Instance;
		}
	}
}
