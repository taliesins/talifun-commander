namespace Talifun.Commander.Command.YouTubeUploader.Command.Settings
{
	public interface IAuthenticationSettings
	{
		string GoogleUsername { get; set; }
		string GooglePassword { get; set; }
		string YouTubeUsername { get; set; }
		string ApplicationName { get; set; }
		string DeveloperKey { get; set; }
	}
}