namespace Talifun.Commander.Command.BoxNetUploader.Command.Settings
{
	public interface IAuthenticationSettings
	{
		string BoxNetApiKey { get; set; }
		string BoxNetUsername { get; set; }
		string BoxNetPassword { get; set; }
	}
}