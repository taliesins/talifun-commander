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

			BindToElement += OnBindToElement;
        }

		private AudioConversionElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is AudioConversionElement)) return;
			var element = e.Element as AudioConversionElement;

			DataModel = new AudioConversionElementPanelDataModel(element);
			this.DataContext = DataModel;

			StartingValues();
		}

		private void StartingValues()
		{
			if (string.IsNullOrEmpty(bitRateComboBox.Text))
			{
				bitRateComboBox.Text = DataModel.Element.BitRate.ToString();
			}
		}
    }
}
