using System.Windows;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.YouTubeUploader.Properties;

namespace Talifun.Commander.Command.YouTubeUploader.Configuration
{
	/// <summary>
	/// Interaction logic for YouTubeUploaderElementPanel.xaml
	/// </summary>
	public partial class YouTubeUploaderElementPanel : ElementPanelBase
	{
		public YouTubeUploaderElementPanel()
        {
			Settings = YouTubeUploaderConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private YouTubeUploaderElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is YouTubeUploaderElement)) return;
			var element = e.Element as YouTubeUploaderElement;

			DataModel = new YouTubeUploaderElementPanelDataModel(element);
			this.DataContext = DataModel;
		}

		private void ApiSignUpYouTubeButton_Click(object sender, RoutedEventArgs e)
		{
			OpenLink(Resource.YouTubeApiSignUpUrl);
		}
	}
}
