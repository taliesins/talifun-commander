namespace Talifun.Commander.Command.PicasaUploader.Command.Settings
{
	public class AuthenticationSettings : IAuthenticationSettings
	{
		public AuthenticationSettings()
		{
			PicasaUsername = "default";
		}
		public string GoogleUsername { get; set; }
		public string GooglePassword { get; set; }
		public string PicasaUsername { get; set; }
		public string ApplicationName { get; set; }
		public string AlbumId { get; set; }
	}
}
