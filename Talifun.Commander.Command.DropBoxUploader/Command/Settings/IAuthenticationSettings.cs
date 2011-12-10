namespace Talifun.Commander.Command.DropBoxUploader.Command.Settings
{
	public interface IAuthenticationSettings
	{
		string DropBoxApiKey { get; set; }
		string DropBoxApiSecret { get; set; }
		string DropBoxUsername { get; set; }
		string DropBoxPassword { get; set; }
	}
}