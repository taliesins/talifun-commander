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

            BindToElement += OnBindToElement;
        }

        private ProjectElement Element { get; set; }

        private void OnBindToElement(object sender, BindToElementEventArgs e)
        {
            if (e.Element == null || !(e.Element is ProjectElement)) return;
			Element = e.Element as ProjectElement;
        	
            this.DataContext = Element;
        }
    }
}