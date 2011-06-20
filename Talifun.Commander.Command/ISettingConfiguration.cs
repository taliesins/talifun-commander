using System.Drawing;

namespace Talifun.Commander.Command
{
    public interface ISettingConfiguration
    {
        string ConversionType { get; }
        string CollectionSettingName { get; }
        string ElementSettingName { get; }
        Image ElementImage { get; }
        Image ElementCollectionImage { get; }
    }
}
