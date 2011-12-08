namespace Talifun.Commander.Command.FlickrUploader.Command.Settings
{
	public interface IAuthenticationSettings
	{
		string FlickrApiKey { get; set; }
		string FlickrApiSecret { get; set; }
		string FlickrFrob { get; set; }
		string FlickrAuthToken { get; set; }
	}
}