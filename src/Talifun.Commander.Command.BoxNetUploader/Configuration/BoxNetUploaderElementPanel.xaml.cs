using System;
using System.Windows;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BoxNet;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.BoxNetUploader.Configuration
{
	/// <summary>
	/// Interaction logic for BoxNetUploaderElementPanel.xaml
	/// </summary>
	public partial class BoxNetUploaderElementPanel : ElementPanelBase
	{
		public BoxNetUploaderElementPanel()
        {
			Settings = BoxNetUploaderConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private BoxNetUploaderElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is BoxNetUploaderElement)) return;
			var element = e.Element as BoxNetUploaderElement;

			DataModel = new BoxNetUploaderElementPanelDataModel(element);
			this.DataContext = DataModel;
		}

		private BoxNetConfiguration GetBoxNetConfiguration()
		{
			var config = BoxNetConfiguration.GetBoxNetConfiguration();
			return config;
		}

		protected GenericNetworkCredentials CheckAuthenticationToken(string username, string password)
		{
			var credentials = new GenericNetworkCredentials
			{
				UserName = username,
				Password = password
			};

			return credentials;
		}

		private void AuthenticateBoxNetButton_Click(object sender, RoutedEventArgs e)
		{
		    authenticateBoxNetButton.IsEnabled = false;
		    authenticateBoxNetLabel.Content = "";
			try
			{
			    var config = GetBoxNetConfiguration();
			    var credentials = CheckAuthenticationToken(DataModel.Element.BoxNetUsername, DataModel.Element.BoxNetPassword);
			    var storage = new CloudStorage();
			    var accessToken = storage.Open(config, credentials);
			    var rootFolder = storage.GetRoot();

			    authenticateBoxNetLabel.Content = "Authentication Successful";
			}
			catch (Exception exception)
			{
			    authenticateBoxNetLabel.Content = "Authentication Failure";
			}
			finally
			{
                authenticateBoxNetButton.IsEnabled = true;
			}
		}
	}
}
