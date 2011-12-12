using System.Windows;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Common.Net.oAuth.Token;
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
			this.DataContext = DataModel;
		}

		private DropBoxConfiguration GetDropBoxConfiguration()
		{
			var config = DropBoxConfiguration.GetStandardConfiguration();
			config.AuthorizationCallBack = Resource.DropBoxOobAuthorizationUrl;
			return config;
		}

		private void ApiSignUpDropBoxTokenButton_Click(object sender, RoutedEventArgs e)
		{
			OpenLink(Resource.DropBoxApiSignUpUrl);
		}

		private void CreateDropBoxRequestTokenButton_Click(object sender, RoutedEventArgs e)
		{
			var config = GetDropBoxConfiguration();
			var requestToken = DropBoxStorageProviderTools.GetDropBoxRequestToken(config, DataModel.Element.DropBoxApiKey, DataModel.Element.DropBoxApiSecret);
			DataModel.Element.DropBoxRequestKey = requestToken.RealToken.TokenKey;
			DataModel.Element.DropBoxRequestSecret = requestToken.RealToken.TokenSecret;
		}

		private void AuthorizeDropBoxRequestTokenButton_Click(object sender, RoutedEventArgs e)
		{
			var config = GetDropBoxConfiguration();
			var requestToken = new DropBoxRequestToken(new OAuthToken(DataModel.Element.DropBoxRequestKey, DataModel.Element.DropBoxRequestSecret));
			var url = DropBoxStorageProviderTools.GetDropBoxAuthorizationUrl(config, requestToken);
			OpenLink(url);
		}

		private void AuthenticateDropBoxRequestTokenButton_Click(object sender, RoutedEventArgs e)
		{
			var config = GetDropBoxConfiguration();
			
			ICloudStorageAccessToken accessToken = null;	
			if (!string.IsNullOrEmpty(DataModel.Element.DropBoxAuthenticationSecret))
			{
				var requestToken = new DropBoxBaseTokenInformation()
				{
					ConsumerKey = DataModel.Element.DropBoxApiKey,
					ConsumerSecret = DataModel.Element.DropBoxApiSecret
				};
				var authenticationToken = new OAuthToken(DataModel.Element.DropBoxAuthenticationKey, DataModel.Element.DropBoxAuthenticationSecret);
				accessToken = new DropBoxToken(authenticationToken, requestToken);
			}
			else
			{
				var requestToken = new DropBoxRequestToken(new OAuthToken(DataModel.Element.DropBoxRequestKey, DataModel.Element.DropBoxRequestSecret));
				accessToken = DropBoxStorageProviderTools.ExchangeDropBoxRequestTokenIntoAccessToken(config, DataModel.Element.DropBoxApiKey, DataModel.Element.DropBoxApiSecret, requestToken);
				DataModel.Element.DropBoxAuthenticationKey = ((DropBoxToken)accessToken).TokenKey;
				DataModel.Element.DropBoxAuthenticationSecret = ((DropBoxToken)accessToken).TokenSecret;
			}

			var accountInformation = DropBoxStorageProviderTools.GetAccountInformation(accessToken);
			displayNameLabel.Content = accountInformation.DisplayName;
			userIdLabel.Content = accountInformation.UserId;
			countryLabel.Content = accountInformation.Country;
			quotaInfoLabel.Content = accountInformation.QuotaInfo.QuotaBytes;
			quotaInfoNormalLabel.Content = accountInformation.QuotaInfo.NormalBytes;
			quotaInfoSharedLabel.Content = accountInformation.QuotaInfo.SharedBytes;
		}
	}
}
