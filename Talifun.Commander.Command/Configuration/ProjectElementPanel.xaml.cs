using System.ComponentModel;
using System.Configuration;
using System.Windows;

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
            if (Element != null)
            {
                Element.PropertyChanged -= OnElementPropertyChanged;
            }

            if (e.Element == null || !(e.Element is ProjectElement)) return;
			Element = e.Element as ProjectElement;
        	
            SaveButton.IsEnabled = false;

            this.DataContext = Element;

            Element.PropertyChanged += OnElementPropertyChanged;
        }

        void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveButton.IsEnabled = true;
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = false;
            Element.CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
        }
    }
}