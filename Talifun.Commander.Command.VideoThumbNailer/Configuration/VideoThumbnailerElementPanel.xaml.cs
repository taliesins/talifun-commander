using System.ComponentModel;
using System.Configuration;
using System.Windows;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.VideoThumbNailer.Configuration
{
    /// <summary>
    /// Interaction logic for VideoThumbnailerElementPanel.xaml
    /// </summary>
    public partial class VideoThumbnailerElementPanel : ElementPanelBase
    {
        public VideoThumbnailerElementPanel()
        {
            Settings = VideoThumbnailerConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private VideoThumbnailerElement Element { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (Element != null)
			{
				Element.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.Element == null || !(e.Element is VideoThumbnailerElement)) return;
			Element = e.Element as VideoThumbnailerElement;

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
