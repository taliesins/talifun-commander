using System.ComponentModel.Composition;

namespace Talifun.Commander.Command.Configuration
{
    [InheritedExport]
    public class ElementCollectionPanelBase : SettingPanelBase
    {
        public virtual void OnBindToElementCollection(CurrentConfigurationElementCollection elementCollection)
        {
            var handler = BindToElementCollection;
            if (handler != null)
            {
                handler(this, new BindToElementCollectionEventArgs(elementCollection));
            }
        }

        public event BindToElementCollectionEventHandler BindToElementCollection;
    }
}
