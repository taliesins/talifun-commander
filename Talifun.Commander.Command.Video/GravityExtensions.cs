using System;

namespace Talifun.Commander.Command.Video
{
	public static class GravityExtensions
	{
		public static string GetOverlayPosition(this Gravity gravity)
		{
			switch (gravity)
			{
				case Gravity.NorthWest:
					return "{0}:{1}";
				case Gravity.North:
					return "(main_w/2)-(overlay_w/2):{1}";
				case Gravity.NorthEast:
					return "main_w-overlay_w-{0}:{1}";
				case Gravity.East:
					return "main_w-overlay_w-{0}:(main_h/2)-(overlay_h/2)";
				case Gravity.SouthEast:
					return "main_w-overlay_w-{0}:main_h-overlay_h-{1}";
				case Gravity.South:
					return "(main_w/2)-(overlay_w/2):main_h-overlay_h-{1}";
				case Gravity.SouthWest:
					return "{0}:main_h-overlay_h-{1}";
				case Gravity.West:
					return "{0}:(main_h/2)-(overlay_h/2)";
				case Gravity.Center:
					return "(main_w/2)-(overlay_w/2):(main_h/2)-(overlay_h/2)";
				default:
					throw new Exception();
			}
		}
	}
}
