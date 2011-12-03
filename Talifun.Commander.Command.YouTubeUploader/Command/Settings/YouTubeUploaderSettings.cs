namespace Talifun.Commander.Command.YouTubeUploader.Command.Settings
{
	public class YouTubeUploaderSettings : IYouTubeUploaderSettings
	{
		public IAuthenticationSettings Authentication { get; set; }
		public IUploadSettings Upload { get; set; }
		public YouTubeMetaData MetaData { get; set; }
	}
}
