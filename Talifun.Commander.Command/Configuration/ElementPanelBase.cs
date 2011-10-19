using System.ComponentModel.Composition;
using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
    [InheritedExport]
    public class ElementPanelBase : SettingPanelBase
    {
        public virtual void OnBindToElement(AppSettingsSection appSettings, CommanderSection commanderSettings, NamedConfigurationElement element)
        {
            var handler = BindToElement;
            if (handler != null)
            {
				handler(this, new BindToElementEventArgs(appSettings, commanderSettings, element));
            }
        }

        public event BindToElementEventHandler BindToElement;
    }
}
