namespace Talifun.Commander.Command.PicasaUploader.Command.Settings
{
	public interface IAuthenticationSettings
	{
		string GoogleUsername { get; set; }
		string GooglePassword { get; set; }
		string PicasaUsername { get; set; }
		string ApplicationName { get; set; }
		string AlbumId { get; set; }
	}
}