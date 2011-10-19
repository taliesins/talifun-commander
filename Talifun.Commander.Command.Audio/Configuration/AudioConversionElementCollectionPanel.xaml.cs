using System.Windows.Forms;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Audio.Configuration
{
	/// <summary>
	/// Interaction logic for AudioConversionElementCollectionPanel.xaml
	/// </summary>
	public partial class AudioConversionElementCollectionPanel : ElementCollectionPanelBase
	{
		public AudioConversionElementCollectionPanel()
		{
			Settings = AudioConversionConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private AudioConversionElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new AudioConversionElementCollectionPanelDataModel(e.AppSettings);
			this.DataContext = DataModel;
		}

		private void fFMpegPathButton_Click(object sender, System.Windows.RoutedEventArgs e)
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
