﻿using System.Windows;
using System.Windows.Forms;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.VideoThumbnailer.Configuration
{
	/// <summary>
	/// Interaction logic for VideoThumbnailerElementCollectionPanel.xaml
	/// </summary>
	public partial class VideoThumbnailerElementCollectionPanel : ElementCollectionPanelBase
	{
		public VideoThumbnailerElementCollectionPanel()
		{
			Settings = VideoThumbnailerConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private VideoThumbnailerElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new VideoThumbnailerElementCollectionPanelDataModel(e.AppSettings);
			this.DataContext = DataModel;
		}

		private void fFMpegPathButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				FileName = fFMpegPathTextBox.Text,
				Multiselect = false
			};

			var result = openFileDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = openFileDialog.FileName;
			fFMpegPathTextBox.Text = foldername;
		}
	}
}
