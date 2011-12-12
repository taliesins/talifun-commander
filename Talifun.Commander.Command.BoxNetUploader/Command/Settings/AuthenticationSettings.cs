namespace Talifun.Commander.Command.BoxNetUploader.Command.Settings
{
	public class AuthenticationSettings : IAuthenticationSettings
	{
		public string BoxNetApiKey { get; set; }
		public string BoxNetUsername { get; set; }
		public string BoxNetPassword { get; set; }
	}
}
