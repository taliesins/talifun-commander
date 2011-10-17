using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

namespace Talifun.Commander.Service
{
    [RunInstaller(true)]
    public partial class CommandServiceInstaller : Installer
    {
        public CommandServiceInstaller()
        {
            InitializeComponent();
        }

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
		public override void Commit(IDictionary savedState)
		{
			base.Commit(savedState);
			if (!EventLog.SourceExists(Properties.Resource.LogSource))
			{
				EventLog.CreateEventSource(Properties.Resource.LogSource, Properties.Resource.LogName);
			}
		}

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
		public override void Uninstall(IDictionary savedState)
		{
			if (EventLog.SourceExists(Properties.Resource.LogSource))
			{
				EventLog.DeleteEventSource(Properties.Resource.LogSource);
			}
			base.Uninstall(savedState);
		}
    }
}
