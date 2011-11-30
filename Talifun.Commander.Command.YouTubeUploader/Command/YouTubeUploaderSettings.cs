namespace Talifun.Commander.Command.YouTubeUploader
{
	public class YouTubeUploaderSettings : IYouTubeUploaderSettings
	{
		public IAuthenticationSettings Authentication { get; set; }
		public IUploadSettings Upload { get; set; }
	}
}
