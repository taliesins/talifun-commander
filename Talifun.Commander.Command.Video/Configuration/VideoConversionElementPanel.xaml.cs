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

			Element.PropertyChanged += OnElementPropertyChanged;

			this.DataContext = Element;
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetSelectedVideoFormat();
			SaveButton.IsEnabled = true;
		}

		private void SaveButtonClick(object sender, RoutedEventArgs e)
		{
			SaveButton.IsEnabled = false;
			Element.CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
		}

		private string GetSelectedValueFromComboBox(ComboBox comboBox)
		{
			var value = comboBox.SelectedValue == null
					? string.Empty
					: comboBox.SelectedValue.ToString();

			return value;
		}

		private T GetSelectedEnumFromComboBox<T>(ComboBox comboBox, T defaultValue)
		{
			var value = defaultValue;
			if (comboBox.SelectedItem is T)
			{
				value = (T)comboBox.SelectedItem;
			}

			return value;
		}

    	private void SetSelectedVideoFormat()
		{
			if (_selectionBoxChanged) return;

			var audioConversionType = GetSelectedEnumFromComboBox(audioConversionTypeComboBox, AudioConversionType.NotSpecified);
			var audioBitRate = GetSelectedValueFromComboBox(bitRateComboBox);
			var audioFrequency = GetSelectedValueFromComboBox(frequencyComboBox);
			var audioChannel = GetSelectedValueFromComboBox(channelComboBox);
			var videoConversionType = GetSelectedEnumFromComboBox(conversionTypeComboBox, VideoConversionType.NotSpecified);
    		var deinterlace = deinterlaceCheckBox.IsChecked;
			var width = widthIntegerUpDown.Value;
			var height = heightIntegerUpDown.Value;
			var aspectRatio = GetSelectedEnumFromComboBox(aspectRatioComboBox, AspectRatio.NotSpecified);
			var frameRate = GetSelectedValueFromComboBox(frameRateComboBox);
			var videoBitRate = GetSelectedValueFromComboBox(videoBitRateComboBox);
			var maxVideoBitRate = GetSelectedValueFromComboBox(maxVideoBitRateComboBox);
			var bufferSize = GetSelectedValueFromComboBox(bufferSizeComboBox);
			var keyFrameInterval = GetSelectedValueFromComboBox(keyFrameIntervalComboBox);
			var minKeyFrameInterval = GetSelectedValueFromComboBox(minKeyFrameIntervalComboBox);

			var audioConversionTypeConverter = new EnumConverter(typeof(AudioConversionType));
			var videoConversionTypeConverter = new EnumConverter(typeof(VideoConversionType));
			var aspectRatioConverter = new EnumConverter(typeof(AspectRatio));
			
			var xmlElements = (ReadOnlyObservableCollection<XmlNode>)commonSettingsComboBox.ItemsSource;
			var selectedXmlElement =
				xmlElements.Where(x =>
					(AudioConversionType)audioConversionTypeConverter.ConvertFromString(x.Attributes["audioConversionType"].Value) == audioConversionType
					&& x.Attributes["audioBitRate"].Value == audioBitRate
					&& x.Attributes["audioFrequency"].Value == audioFrequency
					&& x.Attributes["audioChannel"].Value == audioChannel
					&& (VideoConversionType)videoConversionTypeConverter.ConvertFromString(x.Attributes["videoConversionType"].Value) == videoConversionType
					&& bool.Parse(x.Attributes["deinterlace"].Value) == deinterlace
					&& int.Parse(x.Attributes["width"].Value) == width 
					&& int.Parse(x.Attributes["height"].Value) == height
					&& (AspectRatio)aspectRatioConverter.ConvertFromString(x.Attributes["aspectRatio"].Value) == aspectRatio
					&& x.Attributes["frameRate"].Value == frameRate
					&& x.Attributes["videoBitRate"].Value == videoBitRate
					&& x.Attributes["maxVideoBitRate"].Value == maxVideoBitRate
					&& x.Attributes["bufferSize"].Value == bufferSize
					&& x.Attributes["keyFrameInterval"].Value == keyFrameInterval
					&& x.Attributes["minKeyFrameInterval"].Value == minKeyFrameInterval
					)
				.FirstOrDefault() 
				?? xmlElements.Where(x => x.Attributes["name"].Value == "Custom").First();

			if (commonSettingsComboBox.SelectionBoxItem != selectedXmlElement)
			{
				commonSettingsComboBox.SelectedItem = selectedXmlElement;
			}
		}

		private T GetEnumFromAttribute<T>(XmlNode xmlNode, string attributeName)
		{
			var attributeEnumTypeConverter = new EnumConverter(typeof(T));
			var attributeValue = (T)attributeEnumTypeConverter.ConvertFromString(xmlNode.Attributes[attributeName].Value);

			return attributeValue;
		}

		private int GetIntFromAttribute(XmlNode xmlNode, string attributeName)
		{
			var attributeString = xmlNode.Attributes[attributeName].Value;
			var attributeValue = 0;
			if (!int.TryParse(attributeString, out attributeValue))
			{
				attributeValue = 0;
			}

			return attributeValue;
		}


    	private bool _selectionBoxChanged = false;
		private void CommonSettingsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
    		if (e.AddedItems.Count <= 0) return;

    		_selectionBoxChanged = true;
    		var selectedCommonSetting = e.AddedItems[0] as XmlNode;

			var audioConversionType = GetEnumFromAttribute<AudioConversionType>(selectedCommonSetting, "audioConversionType");
			var audioBitRate = GetIntFromAttribute(selectedCommonSetting, "audioBitRate");			
			var audioFrequency = GetIntFromAttribute(selectedCommonSetting, "audioFrequency");
			var audioChannel = GetIntFromAttribute(selectedCommonSetting, "audioChannel");
			var videoConversionType = GetEnumFromAttribute<VideoConversionType>(selectedCommonSetting, "videoConversionType");
			var deinterlace = selectedCommonSetting.Attributes["deinterlace"].Value == "true";
			var width = GetIntFromAttribute(selectedCommonSetting, "width");
			var height = GetIntFromAttribute(selectedCommonSetting, "height");
			var aspectRatio = GetEnumFromAttribute<AspectRatio>(selectedCommonSetting, "aspectRatio");
			var frameRate = GetIntFromAttribute(selectedCommonSetting, "frameRate");
			var videoBitRate = GetIntFromAttribute(selectedCommonSetting, "videoBitRate");
			var maxVideoBitRate = GetIntFromAttribute(selectedCommonSetting, "maxVideoBitRate");
			var bufferSize = GetIntFromAttribute(selectedCommonSetting, "bufferSize");
			var keyFrameInterval = GetIntFromAttribute(selectedCommonSetting, "keyFrameInterval");
			var minKeyFrameIntervals = GetIntFromAttribute(selectedCommonSetting, "minKeyFrameIntervals");

			Element.AudioConversionType = audioConversionType;
			Element.AudioBitRate = audioBitRate;
			Element.AudioFrequency = audioFrequency;
			Element.AudioChannel = audioChannel;
			Element.Deinterlace = deinterlace;
			Element.VideoConversionType = videoConversionType;
    		Element.Width = width;
			Element.Height = height;
			Element.AspectRatio = aspectRatio;
			Element.FrameRate = frameRate;
			Element.VideoBitRate = videoBitRate;
			Element.MaxVideoBitRate = maxVideoBitRate;
			Element.BufferSize = bufferSize;
			Element.KeyFrameInterval = keyFrameInterval;
			Element.MinKeyFrameInterval = minKeyFrameIntervals;
			
    		_selectionBoxChanged = false;
		}
    }
}
