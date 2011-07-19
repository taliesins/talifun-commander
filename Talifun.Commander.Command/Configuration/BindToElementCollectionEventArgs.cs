using System;

namespace Talifun.Commander.Command.Configuration
{
    public class BindToElementCollectionEventArgs : EventArgs
    {
        public BindToElementCollectionEventArgs(CurrentConfigurationElementCollection elementCollection)
        {
            ElementCollection = elementCollection;
        }

        public CurrentConfigurationElementCollection ElementCollection { get; private set; }
    }
}
