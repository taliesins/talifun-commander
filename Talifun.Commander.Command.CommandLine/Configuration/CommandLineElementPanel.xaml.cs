using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
    /// <summary>
    /// Interaction logic for CommandLineElementPanel.xaml
    /// </summary>
    public partial class CommandLineElementPanel : ElementPanelBase
    {
        public CommandLineElementPanel()
        {
            Settings = CommandLineConfiguration.Instance;
            InitializeComponent();
        }
    }
}
