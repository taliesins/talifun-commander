using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
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

		private void WidthTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			SetSelectedVideoFormat();
		}

		private void HeightTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			SetSelectedVideoFormat();
		}

		private void AspectRatioComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetSelectedVideoFormat();
		}

		private void SetSelectedVideoFormat()
		{
			if (_selectionBoxChanged) return;

			var textWidth = widthTextBox.Text;
			int width;
			if (!int.TryParse(textWidth, out width))
			{
				width = 0;
			}

			var textHeight = heightTextBox.Text;
			int height;
			if (!int.TryParse(textHeight, out height))
			{
				height = 0;
			}

			var aspectRatio = AspectRatio.NotSpecified;
			if (aspectRatioComboBox.SelectedItem is AspectRatio)
			{
				aspectRatio = (AspectRatio)aspectRatioComboBox.SelectedItem;
			}
			
			var xmlElements = (ReadOnlyObservableCollection<XmlNode>)videoFormatComboBox.ItemsSource;
			var aspectRatioConverter = new EnumConverter(typeof(AspectRatio));
			var selectedXmlElement =
				xmlElements.Where(x => 
					int.Parse(x.Attributes["width"].Value) == width 
					&& int.Parse(x.Attributes["height"].Value) == height
					&& (AspectRatio)aspectRatioConverter.ConvertFromString(x.Attributes["aspectRatio"].Value) == aspectRatio)
				.FirstOrDefault() 
				?? xmlElements.Where(x => 
					int.Parse(x.Attributes["width"].Value) == 0 
					&& int.Parse(x.Attributes["height"].Value) == 0
					&& (AspectRatio)aspectRatioConverter.ConvertFromString(x.Attributes["aspectRatio"].Value) == AspectRatio.NotSpecified)
				.First();

			if (videoFormatComboBox.SelectionBoxItem != selectedXmlElement)
			{
				videoFormatComboBox.SelectedItem = selectedXmlElement;
			}
		}


    	private bool _selectionBoxChanged = false;
    	private void VideoFormatComboBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
    		if (e.AddedItems.Count <= 0) return;

    		_selectionBoxChanged = true;
    		var selectedVideoFormat = e.AddedItems[0] as XmlNode;
    		var width = int.Parse(selectedVideoFormat.Attributes["width"].Value);
    		var height = int.Parse(selectedVideoFormat.Attributes["height"].Value);

			var aspectRatioConverter = new EnumConverter(typeof(AspectRatio));

    		var aspectRatio = (AspectRatio) aspectRatioConverter.ConvertFromString(selectedVideoFormat.Attributes["aspectRatio"].Value);

    		if (width > 0 && Element.Width != width)
    		{
    			Element.Width = width;
    		}

    		if (height > 0 && Element.Height != height)
    		{
    			Element.Height = height;
    		}

			if (Element.AspectRatio != aspectRatio)
			{
				Element.AspectRatio = aspectRatio;
			}
    		_selectionBoxChanged = false;
		}


    }
}
