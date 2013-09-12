using System;
using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
    public class BindToElementCollectionEventArgs : EventArgs
    {
        public BindToElementCollectionEventArgs(AppSettingsSection appSettings, CommanderSection commanderSettings, CurrentConfigurationElementCollection elementCollection)
        {
			AppSettings = appSettings;
			CommanderSettings = commanderSettings;
            ElementCollection = elementCollection;
        }

		public AppSettingsSection AppSettings { get; private set; }
		public CommanderSection CommanderSettings { get; private set; }
		public CurrentConfigurationElementCollection ElementCollection { get; private set; }
    }
}
