namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Interaction logic for ProjectElementPanel.xaml
    /// </summary>
    public partial class ProjectElementPanel : ElementPanelBase
    {
        public ProjectElementPanel()
        {
            Settings = ProjectConfiguration.Instance;
            InitializeComponent();
        }
    }
}
