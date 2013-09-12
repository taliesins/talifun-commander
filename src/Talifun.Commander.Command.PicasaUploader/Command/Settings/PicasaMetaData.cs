using System;

namespace Talifun.Commander.Command.PicasaUploader.Command.Settings
{
	public class PicasaMetaData
	{
		public PicasaMetaData()
		{
			AlbumId = "default";
			Title = null;
			Description = null;
			Credit = null;
			Keywords = null;
			Published = null;
			Updated = null;
		}

		public string AlbumId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Credit { get; set; }
		public string Keywords { get; set; }
		public DateTime? Published { get; set; }
		public DateTime? Updated { get; set; }
	}
}
