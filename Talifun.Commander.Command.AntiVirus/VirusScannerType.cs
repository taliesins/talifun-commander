using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus
{
    public enum VirusScannerType
    {
		[DisplayString(ResourceKey = "VirusScannerType_McAfee")]
        NotSpecified,
		[DisplayString(ResourceKey = "VirusScannerType_NotSpecified")]
        McAfee
    }
}