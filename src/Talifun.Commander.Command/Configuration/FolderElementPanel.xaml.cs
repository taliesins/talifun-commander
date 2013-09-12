using System.Windows;
using System.Windows.Forms;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Interaction logic for FolderElementPanel.xaml
    /// </summary>
    public partial class FolderElementPanel : ElementPanelBase
    {
        public FolderElementPanel()
        {
            Settings = FolderConfiguration.Instance;
            InitializeComponent();

            BindToElement += OnBindToElement;
        }

        private FolderElement Element { get; set; }

        private void OnBindToElement(object sender, BindToElementEventArgs e)
        {
            if (e.Element == null || !(e.Element is FolderElement)) return;
            Element = e.Element as FolderElement;

            this.DataContext = Element;
        }

		private void folderToWatchButton_Click(object sender, RoutedEventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog
			                          	{
			                          		SelectedPath = folderToWatchTextBox.Text
			                          	};

			var result = folderBrowserDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = folderBrowserDialog.SelectedPath;
			folderToWatchTextBox.Text = foldername;
		}

		private void workingPathButton_Click(object sender, RoutedEventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog
			{
				SelectedPath = workingPathTextBox.Text
			};

			var result = folderBrowserDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = folderBrowserDialog.SelectedPath;
			workingPathTextBox.Text = foldername;
		}

		private void completedPathButton_Click(object sender, RoutedEventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog
			{
				SelectedPath = completedPathTextBox.Text
			};

			var result = folderBrowserDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = folderBrowserDialog.SelectedPath;
			completedPathTextBox.Text = foldername;
		}
    }
}
