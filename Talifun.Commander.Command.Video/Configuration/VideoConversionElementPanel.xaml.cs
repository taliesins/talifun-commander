using System.ComponentModel;
using System.Configuration;
using System.Windows;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Video.Configuration
{
    /// <summary>
    /// Interaction logic for VideoConversionElementPanel.xaml
    /// </summary>
    public partial class VideoConversionElementPanel : ElementPanelBase
    {
        public VideoConversionElementPanel()
        {
            Settings = VideoConversionConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private VideoConversionElement Element { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (Element != null)
			{
				Element.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.Element == null || !(e.Element is VideoConversionElement)) return;
			Element = e.Element as VideoConversionElement;

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
