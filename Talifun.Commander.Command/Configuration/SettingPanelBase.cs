using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Talifun.Commander.Command.Configuration
{
    [InheritedExport]
    public class SettingPanelBase : UserControl
    {
        public ISettingConfiguration Settings { get; protected set; }
    }
}
