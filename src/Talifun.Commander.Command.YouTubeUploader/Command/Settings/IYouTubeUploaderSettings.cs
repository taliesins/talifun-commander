namespace Talifun.Commander.Command.YouTubeUploader.Command.Settings
{
	public interface IYouTubeUploaderSettings
	{
		IAuthenticationSettings Authentication { get; set; }
		IUploadSettings Upload { get; set; }
		YouTubeMetaData MetaData { get; set; }
	}
}