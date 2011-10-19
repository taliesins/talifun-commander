using Talifun.Commander.Command.Configuration;

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
    }
}
