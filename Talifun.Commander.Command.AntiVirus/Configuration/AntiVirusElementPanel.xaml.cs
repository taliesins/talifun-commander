using System.ComponentModel;
using System.Configuration;
using System.Windows;
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

			BindToElement += OnBindToElement;
        }

		private AntiVirusElement Element { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (Element != null)
			{
				Element.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.Element == null || !(e.Element is AntiVirusElement)) return;
			Element = e.Element as AntiVirusElement;

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
