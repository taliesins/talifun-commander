using Talifun.Commander.Command.AntiVirus.Configuration;

namespace Talifun.Commander.Command.AntiVirus
{
	public class AntiVirusService : CommandServiceBase<AntiVirusSaga>
	{
		public override ISettingConfiguration Settings
		{
			get { return AntiVirusConfiguration.Instance; }
		}
	}
}
