using System;
using System.Net;
using System.Windows;
using FlickrNet;
using Talifun.Commander.Command.Configuration;

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

		private void CreateFlickerFrobButton_Click(object sender, RoutedEventArgs e)
		{
			var flickrService = GetFlickrService(null);
			DataModel.Element.FlickrFrob = flickrService.AuthGetFrob();
		}

		private void AuthorizeFlickerFrobButton_Click(object sender, RoutedEventArgs e)
		{
			var flickrService = GetFlickrService(null);
			var url = flickrService.AuthCalcUrl(DataModel.Element.FlickrFrob, FlickrNet.AuthLevel.Write);
			OpenLink(url);
		}

		private void AuthenticateFlickerFrobButton_Click(object sender, RoutedEventArgs e)
		{
			Auth authenticationToken = null;
			if (string.IsNullOrEmpty(DataModel.Element.FlickrAuthToken))
			{
				var flickrService = GetFlickrService(null);
				authenticationToken = flickrService.AuthGetToken(DataModel.Element.FlickrFrob);
				
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

		private void OpenLink(string url)
		{
			try
			{
				System.Diagnostics.Process.Start(url);
			}
			catch (Exception exception)
			{

				// System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.  
				// It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,
				// then attempt to open the URL in IE instead.
				if (exception.GetType().ToString() != "System.ComponentModel.Win32Exception")
				{
					// sometimes throws exception so we have to just ignore
					// this is a common .NET bug that no one online really has a great reason for so now we just need to try to open
					// the URL using IE if we can.
					
					var startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", url);
					System.Diagnostics.Process.Start(startInfo);
					startInfo = null;					
				}
				else
				{
					throw;
				}
			}
		}
	}
}
