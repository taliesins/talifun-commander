using System;
using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
    public class BindToElementEventArgs : EventArgs
    {
		public BindToElementEventArgs(AppSettingsSection appSettings, CommanderSection commanderSettings, NamedConfigurationElement element)
        {
			AppSettings = appSettings;
			CommanderSettings = commanderSettings;
            Element = element;
        }

		public AppSettingsSection AppSettings { get; private set; }
		public CommanderSection CommanderSettings { get; private set; }
        public NamedConfigurationElement Element { get; private set; }
    }
}
