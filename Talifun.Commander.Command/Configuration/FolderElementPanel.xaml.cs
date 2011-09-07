using System.ComponentModel;
using System.Configuration;
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
            if (Element != null)
            {
                Element.PropertyChanged -= OnElementPropertyChanged;
            }

            if (e.Element == null || !(e.Element is FolderElement)) return;
            Element = e.Element as FolderElement;

            SaveButton.IsEnabled = false;

            this.DataContext = Element;

            Element.PropertyChanged += OnElementPropertyChanged;
        }

        void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveButton.IsEnabled = true;
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = false;
            Element.CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
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
