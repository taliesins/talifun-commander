using System.ComponentModel;
using System.Configuration;
using System.Windows;

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

            BindToElement += OnBindToElement;
        }

        private FolderElement Element { get; set; }

        private void OnBindToElement(object sender, BindToElementEventArgs e)
        {
            if (Element != null)
            {
                Element.PropertyChanged -= OnElementPropertyChanged;
            }

            if (e.Element == null || !(e.Element is FolderElement)) return;
            Element = e.Element as FolderElement;

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
