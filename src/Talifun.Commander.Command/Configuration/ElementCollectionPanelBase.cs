using System.ComponentModel.Composition;
using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
    [InheritedExport]
    public class ElementCollectionPanelBase : SettingPanelBase
    {
        public virtual void OnBindToElementCollection(AppSettingsSection appSettings, CommanderSection commanderSettings, CurrentConfigurationElementCollection elementCollection)
        {
            var handler = BindToElementCollection;
            if (handler != null)
            {
				handler(this, new BindToElementCollectionEventArgs(appSettings, commanderSettings, elementCollection));
            }
        }

        public event BindToElementCollectionEventHandler BindToElementCollection;
    }
}
