using System;
using System.Drawing;

namespace Talifun.Commander.Command
{
    public interface ISettingConfiguration
    {
        string ConversionType { get; }
        string ElementSettingName { get; }
        string ElementCollectionSettingName { get; }
        Type ElementCollectionType { get; }
        Type ElementType { get; }
        Bitmap ElementImage { get; }
        Bitmap ElementCollectionImage { get; }
    }
}
