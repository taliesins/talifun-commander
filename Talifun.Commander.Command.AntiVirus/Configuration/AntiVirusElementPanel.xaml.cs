using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
    /// <summary>
    /// Interaction logic for AntiVirusElementPanel.xaml
    /// </summary>
    public partial class AntiVirusElementPanel : ElementPanelBase
    {
        public AntiVirusElementPanel()
        {
            Settings = AntiVirusConfiguration.Instance;
            InitializeComponent();
        }
    }
}
