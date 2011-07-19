using System;

namespace Talifun.Commander.Command.Configuration
{
    public class BindToElementEventArgs : EventArgs
    {
        public BindToElementEventArgs(NamedConfigurationElement element)
        {
            Element = element;
        }

        public NamedConfigurationElement Element { get; private set; }
    }
}
