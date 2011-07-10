namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Interaction logic for FolderElementPanel.xaml
    /// </summary>
    public partial class FolderElementPanel : ElementPanelBase
    {
        public FolderElementPanel()
        {
            Settings = FolderConfiguration.Instance;
            InitializeComponent();
        }
    }
}
