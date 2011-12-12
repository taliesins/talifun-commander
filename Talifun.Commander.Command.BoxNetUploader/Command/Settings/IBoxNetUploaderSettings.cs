namespace Talifun.Commander.Command.BoxNetUploader.Command.Settings
{
	public interface IBoxNetUploaderSettings
	{
		IAuthenticationSettings Authentication { get; set; }
		string Folder { get; set; }
	}
}