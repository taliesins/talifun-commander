namespace Talifun.Commander.Command.PicasaUploader.Command.Settings
{
	public class PicasaUploaderSettings : IPicasaUploaderSettings
	{
		public IAuthenticationSettings Authentication { get; set; }
		public IUploadSettings Upload { get; set; }
		public PicasaMetaData MetaData { get; set; }
	}
}
