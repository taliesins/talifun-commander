﻿using System.Collections.ObjectModel;
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
			DataModel.PropertyChanged += OnElementPropertyChanged;

			this.DataContextChanged += OnVideoConversionElementPanelDataContextChanged;
			this.DataContext = DataModel;
		}

		private void OnVideoConversionElementPanelDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			SetSelectedVideoFormat();
		}

		private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
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
			var videoBitRate = GetSelectedValueFromComboBox(videoBitRateComboBox);
			var maxVideoBitRate = GetSelectedValueFromComboBox(maxVideoBitRateComboBox);

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
					&& x.Attributes["videoBitRate"].Value == videoBitRate
					&& x.Attributes["maxVideoBitRate"].Value == maxVideoBitRate
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
			var videoBitRate = GetIntFromAttribute(selectedCommonSetting, "videoBitRate");
			var maxVideoBitRate = GetIntFromAttribute(selectedCommonSetting, "maxVideoBitRate");

			if (audioConversionType != AudioConversionType.NotSpecified)
			{
				DataModel.Element.AudioConversionType = audioConversionType;
			}

			if (audioBitRate != 0)
			{
				DataModel.Element.AudioBitRate = audioBitRate;
			}

			if (audioFrequency != 0)
			{
				DataModel.Element.AudioFrequency = audioFrequency;
			}

			if (audioChannel != 0)
			{
				DataModel.Element.AudioChannel = audioChannel;
			}

			DataModel.Element.Deinterlace = deinterlace;

			if (videoConversionType != VideoConversionType.NotSpecified)
			{
				DataModel.Element.VideoConversionType = videoConversionType;
			}

			if (width != 0)
			{
				DataModel.Element.Width = width;
			}

			if (height != 0)
			{
				DataModel.Element.Height = height;
			}

			if (aspectRatio != AspectRatio.NotSpecified)
			{
				DataModel.Element.AspectRatio = aspectRatio;
			}

			if (videoBitRate != 0)
			{
				DataModel.Element.VideoBitRate = videoBitRate;
			}

			if (maxVideoBitRate != 0)
			{
				DataModel.Element.MaxVideoBitRate = maxVideoBitRate;
			}

			_selectionBoxChanged = false;
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

		private void introPathButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				FileName = introPathTextBox.Text,
				Multiselect = false
			};

			var result = openFileDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = openFileDialog.FileName;
			introPathTextBox.Text = foldername;
		}

		private void outtroPathButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				FileName = outtroPathTextBox.Text,
				Multiselect = false
			};

			var result = openFileDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = openFileDialog.FileName;
			outtroPathTextBox.Text = foldername;
		}
    }
}
