using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.DropBoxUploader.Configuration
{
	/// <summary>
	/// Interaction logic for DropBoxUploaderElementCollectionPanel.xaml
	/// </summary>
	public partial class DropBoxUploaderElementCollectionPanel : ElementCollectionPanelBase
	{
		public DropBoxUploaderElementCollectionPanel()
		{
			Settings = DropBoxUploaderConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private DropBoxUploaderElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new DropBoxUploaderElementCollectionPanelDataModel(e.AppSettings);
			this.DataContext = DataModel;
		}
	}
}
