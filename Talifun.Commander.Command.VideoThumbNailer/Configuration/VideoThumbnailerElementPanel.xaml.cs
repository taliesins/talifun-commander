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
        }
    }
}
