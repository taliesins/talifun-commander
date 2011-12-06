namespace Talifun.Commander.Command.PicasaUploader.Command.Settings
{
	public interface IPicasaUploaderSettings
	{
		IAuthenticationSettings Authentication { get; set; }
		IUploadSettings Upload { get; set; }
		PicasaMetaData MetaData { get; set; }
	}
}