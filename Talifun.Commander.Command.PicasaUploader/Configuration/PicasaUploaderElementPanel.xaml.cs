using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.PicasaUploader.Configuration
{
	/// <summary>
	/// Interaction logic for PicasaUploaderElementPanel.xaml
	/// </summary>
	public partial class PicasaUploaderElementPanel : ElementPanelBase
	{
		public PicasaUploaderElementPanel()
        {
			Settings = PicasaUploaderConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private PicasaUploaderElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is PicasaUploaderElement)) return;
			var element = e.Element as PicasaUploaderElement;

			DataModel = new PicasaUploaderElementPanelDataModel(element);
			this.DataContext = DataModel;
		}
	}
}
