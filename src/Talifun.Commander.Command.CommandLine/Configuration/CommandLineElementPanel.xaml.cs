using System.Windows;
using System.Windows.Forms;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
    /// <summary>
    /// Interaction logic for CommandLineElementPanel.xaml
    /// </summary>
    public partial class CommandLineElementPanel : ElementPanelBase
    {
        public CommandLineElementPanel()
        {
            Settings = CommandLineConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private CommandLineElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is CommandLineElement)) return;
			var element = e.Element as CommandLineElement;

			DataModel = new CommandLineElementPanelDataModel(element);
			this.DataContext = DataModel;
		}

		private void commandPathButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				FileName = commandPathTextBox.Text,
				Multiselect = false
			};

			var result = openFileDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = openFileDialog.FileName;
			commandPathTextBox.Text = foldername;
		}
    }
}
