namespace Talifun.Commander.Command.DropBoxUploader.Command.Settings
{
	public class DropBoxUploaderSettings : IDropBoxUploaderSettings
	{
		public IAuthenticationSettings Authentication { get; set; }
	}
}
