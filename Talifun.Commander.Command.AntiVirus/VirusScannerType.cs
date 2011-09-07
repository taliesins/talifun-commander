using Talifun.Commander.UI;

namespace Talifun.Commander.Command.AntiVirus
{
    public enum VirusScannerType
    {
		[LocalizableDescription("VirusScannerType_McAfee", typeof(Properties.Resource))]
        NotSpecified,
		[LocalizableDescription("VirusScannerType_NotSpecified", typeof(Properties.Resource))]
        McAfee
    }
}