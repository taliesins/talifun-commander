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

		private VideoThumbnailerElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is VideoThumbnailerElement)) return;
			var element = e.Element as VideoThumbnailerElement;

			DataModel = new VideoThumbnailerElementPanelDataModel(element);
			this.DataContext = DataModel;
		}
    }
}
