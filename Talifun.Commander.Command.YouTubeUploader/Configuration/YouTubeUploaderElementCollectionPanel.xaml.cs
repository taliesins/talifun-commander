using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.YouTubeUploader.Configuration
{
	/// <summary>
	/// Interaction logic for YouTubeUploaderElementCollectionPanel.xaml
	/// </summary>
	public partial class YouTubeUploaderElementCollectionPanel : ElementCollectionPanelBase
	{
		public YouTubeUploaderElementCollectionPanel()
		{
			Settings = YouTubeUploaderConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private YouTubeUploaderElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new YouTubeUploaderElementCollectionPanelDataModel(e.AppSettings);
			this.DataContext = DataModel;
		}
	}
}
