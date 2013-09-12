using System;
using System.Windows;
using Google.GData.Client;
using Google.GData.Photos;
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

		private void AuthenticatePicasaButton_Click(object sender, RoutedEventArgs e)
		{
		    authenticatePicasaButton.IsEnabled = false;
		    authenticatePicasaLabel.Content = "";
			try
			{
			    var query = new PhotoQuery(PicasaQuery.CreatePicasaUri(DataModel.Element.PicasaUsername));
			    query.NumberToRetrieve = 1;

			    var picasaService = new PicasaService(DataModel.Element.ApplicationName);
			    picasaService.setUserCredentials(DataModel.Element.GoogleUsername, DataModel.Element.GooglePassword);

			    var feed = picasaService.Query(query);

			    authenticatePicasaLabel.Content = "Authentication Successful";
			}
			catch (Exception exception)
			{
			    authenticatePicasaLabel.Content = "Authentication Failure";
			}
			finally
			{
                authenticatePicasaButton.IsEnabled = true;
			}
		}
	}
}
