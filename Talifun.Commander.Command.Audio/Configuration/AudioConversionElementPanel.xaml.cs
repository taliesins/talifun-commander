using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Audio.Configuration
{
    /// <summary>
    /// Interaction logic for AudioConversionElementPanel.xaml
    /// </summary>
    public partial class AudioConversionElementPanel : ElementPanelBase
    {
        public AudioConversionElementPanel()
        {
            Settings = AudioConversionConfiguration.Instance;
            InitializeComponent();
        }
    }
}
