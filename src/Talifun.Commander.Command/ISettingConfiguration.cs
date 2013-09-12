using System;
using System.Windows.Media.Imaging;

namespace Talifun.Commander.Command
{
    public interface ISettingConfiguration
    {
        string ConversionType { get; }
        string ElementSettingName { get; }
        string ElementCollectionSettingName { get; }
        Type ElementCollectionType { get; }
        Type ElementType { get; }
        BitmapSource ElementImage { get; }
        BitmapSource ElementCollectionImage { get; }
    }
}
