using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.UI;
using ComboBox = System.Windows.Controls.ComboBox;

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

		private VideoConversionElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (DataModel != null)
			{
				DataModel.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.Element == null || !(e.Element is VideoConversionElement)) return;
			var element = e.Element as VideoConversionElement;

			DataModel = new VideoConversionElementPanelDataModel(element);
			this.DataContext = DataModel;

			DataModel.PropertyChanged += OnElementPropertyChanged;

			StartingValues();
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetSelectedVideoFormat();
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

			DataModel.Element.AudioConversionType = audioConversionType;
			DataModel.Element.AudioBitRate = audioBitRate;
			DataModel.Element.AudioFrequency = audioFrequency;
			DataModel.Element.AudioChannel = audioChannel;
			DataModel.Element.Deinterlace = deinterlace;
			DataModel.Element.VideoConversionType = videoConversionType;
			DataModel.Element.Width = width;
			DataModel.Element.Height = height;
			DataModel.Element.AspectRatio = aspectRatio;
			DataModel.Element.FrameRate = frameRate;
			DataModel.Element.VideoBitRate = videoBitRate;
			DataModel.Element.MaxVideoBitRate = maxVideoBitRate;
			DataModel.Element.BufferSize = bufferSize;
			DataModel.Element.KeyFrameInterval = keyFrameInterval;
			DataModel.Element.MinKeyFrameInterval = minKeyFrameIntervals;
			
    		_selectionBoxChanged = false;
		}

    	private void StartingValues()
		{
			if (string.IsNullOrEmpty(commonSettingsComboBox.Text))
			{
				commonSettingsComboBox.Text = "Custom";
			}

			if (string.IsNullOrEmpty(bitRateComboBox.Text))
			{
				bitRateComboBox.Text = DataModel.Element.AudioBitRate.ToString();
			}

			if (string.IsNullOrEmpty(frequencyComboBox.Text))
			{
				frequencyComboBox.Text = DataModel.Element.AudioFrequency.ToString();
			}

			if (string.IsNullOrEmpty(channelComboBox.Text))
			{
				channelComboBox.Text = DataModel.Element.AudioChannel.ToString();
			}

			if (string.IsNullOrEmpty(frameRateComboBox.Text))
			{
				frameRateComboBox.Text = DataModel.Element.FrameRate.ToString();
			}
			
			if (string.IsNullOrEmpty(videoBitRateComboBox.Text))
			{
				videoBitRateComboBox.Text = DataModel.Element.VideoBitRate.ToString();
			}

			if (string.IsNullOrEmpty(maxVideoBitRateComboBox.Text))
			{
				maxVideoBitRateComboBox.Text = DataModel.Element.MaxVideoBitRate.ToString();
			}

			if (string.IsNullOrEmpty(bufferSizeComboBox.Text))
			{
				bufferSizeComboBox.Text = DataModel.Element.BufferSize.ToString();
			}

			if (string.IsNullOrEmpty(keyFrameIntervalComboBox.Text))
			{
				keyFrameIntervalComboBox.Text = DataModel.Element.KeyFrameInterval.ToString();
			}

			if (string.IsNullOrEmpty(minKeyFrameIntervalComboBox.Text))
			{
				minKeyFrameIntervalComboBox.Text = DataModel.Element.MinKeyFrameInterval.ToString();
			}
		}

		private void watermarkPathButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				FileName = watermarkPathTextBox.Text,
				Multiselect = false
			};

			var result = openFileDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = openFileDialog.FileName;
			watermarkPathTextBox.Text = foldername;
		}
    }
}
