using System.Windows;
using System.Windows.Forms;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Video.Configuration
{
	/// <summary>
	/// Interaction logic for VideoConversionElementCollectionPanel.xaml
	/// </summary>
	public partial class VideoConversionElementCollectionPanel : ElementCollectionPanelBase
	{
		public VideoConversionElementCollectionPanel()
		{
			Settings = VideoConversionConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private VideoConversionElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new VideoConversionElementCollectionPanelDataModel(e.AppSettings);
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

		private void flvTool2PathButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				FileName = flvTool2PathTextBox.Text,
				Multiselect = false
			};

			var result = openFileDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = openFileDialog.FileName;
			flvTool2PathTextBox.Text = foldername;
		}
	}
}
