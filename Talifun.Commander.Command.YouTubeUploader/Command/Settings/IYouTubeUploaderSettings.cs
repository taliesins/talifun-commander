namespace Talifun.Commander.Command.YouTubeUploader.Command.Settings
{
	public interface IYouTubeUploaderSettings
	{
		IAuthenticationSettings Authentication { get; set; }
		IVideoMetaData VideoMetaData { get; set; }
		IUploadSettings Upload { get; set; }
	}
}