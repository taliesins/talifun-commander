using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Talifun.Commander.UI
{
	/// <summary>
	/// This class simply takes an enum and uses some reflection to obtain
	/// the friendly name for the enum. Where the friendlier name is
	/// obtained using the LocalizableDescriptionAttribute, which holds the localized
	/// value read from the resource file for the enum
	/// </summary>
	[ValueConversion(typeof(object), typeof(String))]
	public class EnumToFriendlyNameConverter : IValueConverter
	{
		#region IValueConverter implementation

		/// <summary>
		/// Convert value for binding from source object
		/// </summary>
		public object Convert(object value, Type targetType,
				object parameter, CultureInfo culture)
		{
			// To get around the stupid WPF designer bug
			if (value != null)
			{
				FieldInfo fi = value.GetType().GetField(value.ToString());

				// To get around the stupid WPF designer bug
				if (fi != null)
				{
					var attributes =
						(LocalizableDescriptionAttribute[])
			fi.GetCustomAttributes(typeof
			(LocalizableDescriptionAttribute), false);

					return ((attributes.Length > 0) &&
							(!String.IsNullOrEmpty(attributes[0].Description)))
							   ?
								   attributes[0].Description
							   : value.ToString();
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// ConvertBack value from binding back to source object
		/// </summary>
		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new Exception("Cant convert back");
		}
		#endregion
	}
}
