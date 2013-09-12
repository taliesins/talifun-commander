namespace Talifun.Commander.Command.DropBoxUploader.Command.Settings
{
	public interface IDropBoxUploaderSettings
	{
		IAuthenticationSettings Authentication { get; set; }
		string Folder { get; set; }
	}
}