using System;
using System.Windows;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.DropBoxUploader.Properties;

namespace Talifun.Commander.Command.DropBoxUploader.Configuration
{
	/// <summary>
	/// Interaction logic for DropBoxUploaderElementPanel.xaml
	/// </summary>
	public partial class DropBoxUploaderElementPanel : ElementPanelBase
	{
		public DropBoxUploaderElementPanel()
        {
			Settings = DropBoxUploaderConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private DropBoxUploaderElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is DropBoxUploaderElement)) return;
			var element = e.Element as DropBoxUploaderElement;

			DataModel = new DropBoxUploaderElementPanelDataModel(element);
			DataContext = DataModel;
		}

		private DropBoxConfiguration GetDropBoxConfiguration()
		{
			var config = DropBoxConfiguration.GetStandardConfiguration();
			config.AuthorizationCallBack = new Uri(Resource.DropBoxOobAuthorizationUrl);
            config.APIVersion = DropBoxAPIVersion.V1;
			return config;
		}

		private void ApiSignUpDropBoxTokenButton_Click(object sender, RoutedEventArgs e)
		{
			OpenLink(Resource.DropBoxApiSignUpUrl);
		}

		private void CreateDropBoxRequestTokenButton_Click(object sender, RoutedEventArgs e)
		{
		    createDropBoxRequestTokenButton.IsEnabled = false;
		    DataModel.Element.DropBoxRequestKey = "";
		    DataModel.Element.DropBoxRequestSecret = "";
		    try
		    {
		        var config = GetDropBoxConfiguration();
		        var requestToken = DropBoxStorageProviderTools.GetDropBoxRequestToken(config, DataModel.Element.DropBoxApiKey,
		                                                                              DataModel.Element.DropBoxApiSecret);
		        DataModel.Element.DropBoxRequestKey = requestToken.GetTokenKey();
		        DataModel.Element.DropBoxRequestSecret = requestToken.GetTokenSecret();
		    }
		    finally
		    {
                createDropBoxRequestTokenButton.IsEnabled = true;
		    }
		}

		private void AuthorizeDropBoxRequestTokenButton_Click(object sender, RoutedEventArgs e)
		{
		    authorizeDropBoxRequestTokenButton.IsEnabled = false;
		    try
		    {
		        var config = GetDropBoxConfiguration();
		        var requestToken = DropBoxExtensions.GetDropBoxRequestToken(DataModel.Element.DropBoxRequestKey,
		                                                                    DataModel.Element.DropBoxRequestSecret);
		        var url = DropBoxStorageProviderTools.GetDropBoxAuthorizationUrl(config, requestToken);
		        OpenLink(url);
		    }
		    finally
		    {
                authorizeDropBoxRequestTokenButton.IsEnabled = true;
		    }
		}

		private void AuthenticateDropBoxRequestTokenButton_Click(object sender, RoutedEventArgs e)
		{
		    authenticateDropBoxRequestTokenButton.IsEnabled = false;
            displayNameLabel.Content = "";
            userIdLabel.Content = "";
            countryLabel.Content = "";
            quotaInfoLabel.Content = "";
            quotaInfoNormalLabel.Content = "";
            quotaInfoSharedLabel.Content = "";
		    try
		    {
		        ICloudStorageAccessToken accessToken = null;
		        if (!string.IsNullOrEmpty(DataModel.Element.DropBoxAuthenticationSecret))
		        {
		            accessToken = DropBoxExtensions.GetDropBoxAccessToken(DataModel.Element.DropBoxAuthenticationKey,
		                                                                  DataModel.Element.DropBoxAuthenticationSecret,
		                                                                  DataModel.Element.DropBoxApiKey,
		                                                                  DataModel.Element.DropBoxApiSecret);
		        }
		        else
		        {
		            var config = GetDropBoxConfiguration();
		            var requestToken = DropBoxExtensions.GetDropBoxRequestToken(DataModel.Element.DropBoxRequestKey,
		                                                                        DataModel.Element.DropBoxRequestSecret);
		            accessToken = DropBoxStorageProviderTools.ExchangeDropBoxRequestTokenIntoAccessToken(config,
		                                                                                                 DataModel.Element
		                                                                                                          .DropBoxApiKey,
		                                                                                                 DataModel.Element
		                                                                                                          .DropBoxApiSecret,
		                                                                                                 requestToken);

		            DataModel.Element.DropBoxAuthenticationKey = accessToken.GetTokenKey();
		            DataModel.Element.DropBoxAuthenticationSecret = accessToken.GetTokenSecret();
		        }

		        var accountInformation = DropBoxStorageProviderTools.GetAccountInformation(accessToken);
		        displayNameLabel.Content = accountInformation.DisplayName;
		        userIdLabel.Content = accountInformation.UserId;
		        countryLabel.Content = accountInformation.Country;
		        quotaInfoLabel.Content = accountInformation.QuotaInfo.QuotaBytes;
		        quotaInfoNormalLabel.Content = accountInformation.QuotaInfo.NormalBytes;
		        quotaInfoSharedLabel.Content = accountInformation.QuotaInfo.SharedBytes;
		    }
		    finally
		    {
                authenticateDropBoxRequestTokenButton.IsEnabled = true;
		    }
		}
	}
}
