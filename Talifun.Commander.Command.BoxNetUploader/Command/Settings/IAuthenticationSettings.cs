namespace Talifun.Commander.Command.BoxNetUploader.Command.Settings
{
	public interface IAuthenticationSettings
	{
		string BoxNetUsername { get; set; }
		string BoxNetPassword { get; set; }
	}
}