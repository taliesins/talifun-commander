using System;
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

		public void OpenLink(string url)
		{
			try
			{
				System.Diagnostics.Process.Start(url);
			}
			catch (Exception exception)
			{

				// System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.  
				// It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,
				// then attempt to open the URL in IE instead.
				if (exception.GetType().ToString() != "System.ComponentModel.Win32Exception")
				{
					// sometimes throws exception so we have to just ignore
					// this is a common .NET bug that no one online really has a great reason for so now we just need to try to open
					// the URL using IE if we can.

					var startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", url);
					System.Diagnostics.Process.Start(startInfo);
					startInfo = null;
				}
				else
				{
					throw;
				}
			}
		}
    }
}
