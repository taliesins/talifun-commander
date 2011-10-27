using System.Windows.Forms;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Image.Configuration
{
    /// <summary>
    /// Interaction logic for ImageConversionElementPanel.xaml
    /// </summary>
    public partial class ImageConversionElementPanel : ElementPanelBase
    {
        public ImageConversionElementPanel()
        {
            Settings = ImageConversionConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private ImageConversionElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is ImageConversionElement)) return;
			var element = e.Element as ImageConversionElement;

			DataModel = new ImageConversionElementPanelDataModel(element);
			this.DataContext = DataModel;
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
