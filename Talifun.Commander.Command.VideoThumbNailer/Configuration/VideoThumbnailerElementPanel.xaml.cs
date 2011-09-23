using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.VideoThumbNailer.Configuration
{
    /// <summary>
    /// Interaction logic for VideoThumbnailerElementPanel.xaml
    /// </summary>
    public partial class VideoThumbnailerElementPanel : ElementPanelBase
    {
        public VideoThumbnailerElementPanel()
        {
            Settings = VideoThumbnailerConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private VideoThumbnailerElement Element { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is VideoThumbnailerElement)) return;
			Element = e.Element as VideoThumbnailerElement;

			this.DataContext = Element;
		}
    }
}
