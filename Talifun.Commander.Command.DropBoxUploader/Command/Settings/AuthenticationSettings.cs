namespace Talifun.Commander.Command.DropBoxUploader.Command.Settings
{
	public class AuthenticationSettings : IAuthenticationSettings
	{
		public string DropBoxApiKey { get; set; }
		public string DropBoxApiSecret { get; set; }
		public string DropBoxUsername { get; set; }
		public string DropBoxPassword { get; set; }
	}
}
