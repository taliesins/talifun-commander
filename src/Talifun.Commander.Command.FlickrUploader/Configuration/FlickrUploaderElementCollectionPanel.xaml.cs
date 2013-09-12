using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.FlickrUploader.Configuration
{
	/// <summary>
	/// Interaction logic for FlickrUploaderElementCollectionPanel.xaml
	/// </summary>
	public partial class FlickrUploaderElementCollectionPanel : ElementCollectionPanelBase
	{
		public FlickrUploaderElementCollectionPanel()
		{
			Settings = FlickrUploaderConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private FlickrUploaderElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new FlickrUploaderElementCollectionPanelDataModel(e.AppSettings);
			this.DataContext = DataModel;
		}
	}
}
