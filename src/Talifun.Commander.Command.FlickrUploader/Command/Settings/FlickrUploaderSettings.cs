namespace Talifun.Commander.Command.FlickrUploader.Command.Settings
{
	public class FlickrUploaderSettings : IFlickrUploaderSettings
	{
		public IAuthenticationSettings Authentication { get; set; }
		public FlickrMetaData MetaData { get; set; }
	}
}
