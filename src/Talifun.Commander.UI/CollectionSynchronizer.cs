using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Talifun.Commander.UI
{
	public class CollectionSynchronizer
	{
		private readonly INotifyCollectionChanged _source;
		private readonly ObservableCollection<object> _target;
		private readonly IValueConverter _valueConverter;

		public CollectionSynchronizer(INotifyCollectionChanged source, ObservableCollection<object> target, IValueConverter valueConverter)
		{
			_source = source;
			_target = target;
			_valueConverter = valueConverter;
			_source.CollectionChanged += OnSourceCollectionChanged;
		}

		private object Convert(object item)
		{
			return _valueConverter.Convert(item, typeof(object), null, CultureInfo.CurrentCulture);
		}

		private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					for (var i = 0; i < e.NewItems.Count; i++)
					{
						_target.Insert(e.NewStartingIndex + i, Convert(e.NewItems[i]));
					}
					break;

				case NotifyCollectionChangedAction.Move:
					if (e.OldItems.Count == 1)
					{
						_target.Move(e.OldStartingIndex, e.NewStartingIndex);
					}
					else
					{
						var items = _target.Skip(e.OldStartingIndex).Take(e.OldItems.Count).ToList();
						for (var i = 0; i < e.OldItems.Count; i++)
							_target.RemoveAt(e.OldStartingIndex);

						for (var i = 0; i < items.Count; i++)
							_target.Insert(e.NewStartingIndex + i, items[i]);
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					for (var i = 0; i < e.OldItems.Count; i++)
						_target.RemoveAt(e.OldStartingIndex);
					break;

				case NotifyCollectionChangedAction.Replace:
					// remove
					for (var i = 0; i < e.OldItems.Count; i++)
						_target.RemoveAt(e.OldStartingIndex);

					// add
					goto case NotifyCollectionChangedAction.Add;

				case NotifyCollectionChangedAction.Reset:
					_target.Clear();
					for (var i = 0; i < e.NewItems.Count; i++)
						_target.Add(Convert(e.NewItems[i]));
					break;

				default:
					break;
			}
		}

	}
}
