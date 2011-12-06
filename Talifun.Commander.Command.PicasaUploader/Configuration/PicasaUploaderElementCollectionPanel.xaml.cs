using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.PicasaUploader.Configuration
{
	/// <summary>
	/// Interaction logic for PicasaUploaderElementCollectionPanel.xaml
	/// </summary>
	public partial class PicasaUploaderElementCollectionPanel : ElementCollectionPanelBase
	{
		public PicasaUploaderElementCollectionPanel()
		{
			Settings = PicasaUploaderConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private PicasaUploaderElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new PicasaUploaderElementCollectionPanelDataModel(e.AppSettings);
			this.DataContext = DataModel;
		}
	}
}
