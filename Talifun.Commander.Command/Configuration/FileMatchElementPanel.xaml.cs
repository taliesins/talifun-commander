namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Interaction logic for FileMatchElementPanel.xaml
    /// </summary>
    public partial class FileMatchElementPanel : ElementPanelBase
    {
        public FileMatchElementPanel()
        {
            Settings = FileMatchConfiguration.Instance;
            InitializeComponent();
        }
    }
}
