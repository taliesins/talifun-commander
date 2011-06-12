namespace Talifun.Commander.Command
{
    public interface ISettingConfiguration
    {
        string ConversionType { get; }
        string CollectionSettingName { get; }
        string ElementSettingName { get; }
    }
}
