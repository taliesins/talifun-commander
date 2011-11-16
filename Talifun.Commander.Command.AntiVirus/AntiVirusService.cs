using Talifun.Commander.Command.AntiVirus.Command;
using Talifun.Commander.Command.AntiVirus.CommandTester;
using Talifun.Commander.Command.AntiVirus.Configuration;

namespace Talifun.Commander.Command.AntiVirus
{
	public class AntiVirusService : CommandServiceBase<AntiVirusSaga, AntiVirusTesterSaga>
	{
		public override ISettingConfiguration Settings
		{
			get { return AntiVirusConfiguration.Instance; }
		}
	}
}
