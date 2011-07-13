using System.ComponentModel.Composition;

namespace Talifun.Commander.Command.Configuration
{
    [InheritedExport]
    public class ElementPanelBase : SettingPanelBase
    {
        public virtual void OnBindToElement(NamedConfigurationElement element)
        {
            var handler = BindToElement;
            if (handler != null)
            {
                handler(this, new BindToElementEventArgs(element));
            }
        }

        public event BindToElementEventHandler BindToElement;
    }
}
