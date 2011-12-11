namespace Talifun.Commander.Command.DropBoxUploader.Command.Settings
{
	public interface IAuthenticationSettings
	{
		string DropBoxApiKey { get; set; }
		string DropBoxApiSecret { get; set; }
		string DropBoxRequestKey { get; set; }
		string DropBoxRequestSecret { get; set; }
		string DropBoxAuthenticationKey { get; set; }
		string DropBoxAuthenticationSecret { get; set; }
	}
}