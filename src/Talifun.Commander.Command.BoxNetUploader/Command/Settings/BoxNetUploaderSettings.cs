namespace Talifun.Commander.Command.BoxNetUploader.Command.Settings
{
	public class BoxNetUploaderSettings : IBoxNetUploaderSettings
	{
		public IAuthenticationSettings Authentication { get; set; }
		public string Folder { get; set; }
	}
}
