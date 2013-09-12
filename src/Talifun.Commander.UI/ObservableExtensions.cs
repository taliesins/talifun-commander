using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Talifun.Commander.UI
{
	public static class ObservableExtensions
	{
		public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
		{
			return enumerableList != null ? new ObservableCollection<T>(enumerableList) : null;
		}
	}
}
