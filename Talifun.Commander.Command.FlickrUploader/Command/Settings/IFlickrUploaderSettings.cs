namespace Talifun.Commander.Command.FlickrUploader.Command.Settings
{
	public interface IFlickrUploaderSettings
	{
		IAuthenticationSettings Authentication { get; set; }
		FlickrMetaData MetaData { get; set; }
	}
}