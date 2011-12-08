using FlickrNet;

namespace Talifun.Commander.Command.FlickrUploader.Command.Settings
{
	public class FlickrMetaData
	{
		public FlickrMetaData()
		{
			Title = null;
			Description = null;
			Keywords = null;
			IsPublic = false;
			IsFamily = false;
			IsFriend = false;
			ContentType = ContentType.None;
			SafetyLevel = SafetyLevel.None;
			HiddenFromSearch = HiddenFromSearch.None;
		}

		public string Title { get; set; }
		public string Description { get; set; }
		public string Keywords { get; set; }
		public bool IsPublic { get; set; }
		public bool IsFamily { get; set; }
		public bool IsFriend { get; set; }
		public ContentType ContentType { get; set; }
		public SafetyLevel SafetyLevel { get; set; }
		public HiddenFromSearch HiddenFromSearch { get; set; }
	}
}
