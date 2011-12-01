namespace Talifun.Commander.Command.YouTubeUploader.Command.Settings
{
	public class AuthenticationSettings : IAuthenticationSettings
	{
		public string GoogleUsername { get; set; }
		public string GooglePassword { get; set; }
		public string YouTubeUsername { get; set; }
		public string ApplicationName { get; set; }
		public string DeveloperKey { get; set; }
	}
}
