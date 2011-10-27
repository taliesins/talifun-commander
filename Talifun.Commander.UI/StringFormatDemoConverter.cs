using System;
using System.Windows.Data;

namespace Talifun.Commander.UI
{
	[ValueConversion(typeof(string), typeof(string))]
	public class StringFormatDemoConverter : IValueConverter 
	{
		#region IValueConverter Members

		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stringFormat = (string)value;
			var output = string.Empty;

			try
			{
				output = string.Format(stringFormat, parameter);
			} catch(FormatException)
			{
				output = null;
			}

			return output;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
