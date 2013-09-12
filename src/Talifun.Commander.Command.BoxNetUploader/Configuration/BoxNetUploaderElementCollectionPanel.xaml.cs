using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.BoxNetUploader.Configuration
{
	/// <summary>
	/// Interaction logic for BoxNetUploaderElementCollectionPanel.xaml
	/// </summary>
	public partial class BoxNetUploaderElementCollectionPanel : ElementCollectionPanelBase
	{
		public BoxNetUploaderElementCollectionPanel()
		{
			Settings = BoxNetUploaderConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private BoxNetUploaderElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new BoxNetUploaderElementCollectionPanelDataModel(e.AppSettings);
			this.DataContext = DataModel;
		}
	}
}
