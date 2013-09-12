using System.Windows;
using System.Windows.Forms;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Image.Configuration
{
	/// <summary>
	/// Interaction logic for ImageConversionElementCollectionPanel.xaml
	/// </summary>
	public partial class ImageConversionElementCollectionPanel : ElementCollectionPanelBase
	{
		public ImageConversionElementCollectionPanel()
		{
			Settings = ImageConversionConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private ImageConversionElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new ImageConversionElementCollectionPanelDataModel(e.AppSettings);
			this.DataContext = DataModel;
		}

		private void convertPathButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				FileName = convertPathTextBox.Text,
				Multiselect = false
			};

			var result = openFileDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = openFileDialog.FileName;
			convertPathTextBox.Text = foldername;
		}
	}
}
