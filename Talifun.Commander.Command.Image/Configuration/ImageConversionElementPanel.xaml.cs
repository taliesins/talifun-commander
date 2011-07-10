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
        }
    }
}
