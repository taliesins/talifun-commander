namespace Talifun.Commander.Command.FlickrUploader.Command.Settings
{
	public class AuthenticationSettings : IAuthenticationSettings
	{
		public string FlickrApiKey { get; set; }
		public string FlickrApiSecret { get; set; }
		public string FlickrFrob { get; set; }
		public string FlickrAuthToken { get; set; }
	}
}
