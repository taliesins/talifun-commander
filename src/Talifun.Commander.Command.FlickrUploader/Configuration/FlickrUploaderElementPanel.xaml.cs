using System;
using System.Net;
using System.Windows;
using FlickrNet;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.FlickrUploader.Properties;

namespace Talifun.Commander.Command.FlickrUploader.Configuration
{
	/// <summary>
	/// Interaction logic for FlickrUploaderElementPanel.xaml
	/// </summary>
	public partial class FlickrUploaderElementPanel : ElementPanelBase
	{
		public FlickrUploaderElementPanel()
        {
			Settings = FlickrUploaderConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private FlickrUploaderElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is FlickrUploaderElement)) return;
			var element = e.Element as FlickrUploaderElement;

			DataModel = new FlickrUploaderElementPanelDataModel(element);
			this.DataContext = DataModel;
		}

		public Flickr GetFlickrService(string token)
		{
			var key = DataModel.Element.FlickrApiKey;
			var secret = DataModel.Element.FlickrApiSecret;

			Flickr.CacheTimeout = new TimeSpan(1, 0, 0, 0, 0);
			var f = new Flickr(key, secret, token)
			        	{
			        		Proxy = GetProxy()
			        	};
			return f;
		}

		public WebProxy GetProxy()
		{
			return WebProxy.GetDefaultProxy();
		}

		private void ApiSignUpFlickrButton_Click(object sender, RoutedEventArgs e)
		{
			OpenLink(Resource.FlickrApiSignUpUrl);
		}

		private void CreateFlickrFrobButton_Click(object sender, RoutedEventArgs e)
		{
		    createFlickrFrobButton.IsEnabled = false;
		    flickrFrobTextBox.Text = "";
		    try
		    {
		        var flickrService = GetFlickrService(null);
		        flickrFrobTextBox.Text = flickrService.AuthGetFrob();
		    }
		    finally
		    {
                createFlickrFrobButton.IsEnabled = true;
		    }
		}

		private void AuthorizeFlickrFrobButton_Click(object sender, RoutedEventArgs e)
		{
		    authorizeFlickrFrobButton.IsEnabled = false;
		    try
		    {
		        var flickrService = GetFlickrService(null);
		        var url = flickrService.AuthCalcUrl(flickrFrobTextBox.Text, FlickrNet.AuthLevel.Write);
		        OpenLink(url);
		    }
		    finally
		    {
		        authorizeFlickrFrobButton.IsEnabled = true;
		    }
		}

		private void AuthenticateFlickrFrobButton_Click(object sender, RoutedEventArgs e)
		{
		    authenticateFlickrFrobButton.IsEnabled = false;
            fullNameLabel.Content = "";
            userIdLabel.Content = "";
            usernameLabel.Content = "";
		    permissionsLabel.Content = "";
		    try
		    {
		        Auth authenticationToken = null;
		        if (string.IsNullOrEmpty(DataModel.Element.FlickrAuthToken))
		        {
		            var flickrService = GetFlickrService(null);
		            authenticationToken = flickrService.AuthGetToken(flickrFrobTextBox.Text);

		            DataModel.Element.FlickrAuthToken = authenticationToken.Token;
		        }
		        else
		        {
		            var flickrService = GetFlickrService(DataModel.Element.FlickrAuthToken);
		            authenticationToken = flickrService.AuthCheckToken(DataModel.Element.FlickrAuthToken);
		        }

		        fullNameLabel.Content = authenticationToken.User.FullName;
		        userIdLabel.Content = authenticationToken.User.UserId;
		        usernameLabel.Content = authenticationToken.User.UserName;

		        switch (authenticationToken.Permissions)
		        {
		            case AuthLevel.None:
		                permissionsLabel.Content = "None";
		                break;
		            case AuthLevel.Read:
		                permissionsLabel.Content = "Read";
		                break;
		            case AuthLevel.Write:
		                permissionsLabel.Content = "Write";
		                break;
		            case AuthLevel.Delete:
		                permissionsLabel.Content = "Delete";
		                break;
		        }
		    }
		    finally
		    {
                authenticateFlickrFrobButton.IsEnabled = true;
		    }
		}
	}
}
