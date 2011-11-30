namespace Talifun.Commander.Command.YouTubeUploader
{
	public interface IYouTubeUploaderSettings
	{
		IAuthenticationSettings Authentication { get; set; }
		IUploadSettings Upload { get; set; }
	}
}