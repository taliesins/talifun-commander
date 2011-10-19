using System.Windows.Forms;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
	/// <summary>
	/// Interaction logic for AntiVirusElementCollectionPanel.xaml
	/// </summary>
	public partial class AntiVirusElementCollectionPanel : ElementCollectionPanelBase
	{
		public AntiVirusElementCollectionPanel()
		{
			Settings = AntiVirusConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private AntiVirusElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new AntiVirusElementCollectionPanelDataModel(e.AppSettings);
			this.DataContext = DataModel;
		}

		private void mcAfeePathButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				FileName = mcAfeePathTextBox.Text,
				Multiselect = false
			};

			var result = openFileDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = openFileDialog.FileName;
			mcAfeePathTextBox.Text = foldername;
		}
	}
}
