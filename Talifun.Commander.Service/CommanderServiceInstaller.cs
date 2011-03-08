using System.ComponentModel;
using System.Configuration.Install;


namespace Talifun.Commander.Service
{
    [RunInstaller(true)]
    public partial class CommandServiceInstaller : Installer
    {
        public CommandServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
