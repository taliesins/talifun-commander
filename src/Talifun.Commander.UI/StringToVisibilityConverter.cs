using System.Windows.Data;

namespace Talifun.Commander.UI
{
	[ValueConversion(typeof(string), typeof(System.Windows.Visibility))]
	public class StringToVisibilityConverter : IValueConverter 
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stringValue = (string)value;

			return string.IsNullOrEmpty(stringValue) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}
