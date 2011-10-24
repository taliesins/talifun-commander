using System;
using System.Collections;
using System.Collections.Specialized;

namespace Talifun.Commander.UI
{
	public class BindingGroup : IEnumerable, IBindingGroup, INotifyCollectionChanged, IDisposable
	{
		public string Parameter { get; private set; }
		public IEnumerable Items { get; private set; }

		public BindingGroup(IEnumerable items, string parameter)
		{
			Items = items;
			if (items is INotifyCollectionChanged)
			{
				((INotifyCollectionChanged)items).CollectionChanged += OnBindingGroupCollectionChanged;
			}

			Parameter = parameter;
		}

		public Type ElementType
		{
			get
			{
				return GetElementType(Items);
			}
		}

		public static Type GetElementType(IEnumerable enumerable)
		{
			Type elementType = null;
			var enumerableType = enumerable.GetType();
			
			do
			{	
				if (enumerableType.IsGenericType)
				{
					var genericArguments = enumerableType.GetGenericArguments();
					if (genericArguments.Length > 0)
					{
						elementType = genericArguments[0];
					}
				}

				enumerableType = enumerableType.BaseType;
			} while (elementType == null && enumerableType != null);
			
			if (elementType == null)
			{
				var enumItems = enumerable.GetEnumerator();
				if (enumItems.MoveNext())
				{
					if (enumItems.Current != null)
					{
						elementType = enumItems.Current.GetType();
					}
				}
			}
			return elementType;
		}

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}
		#endregion

		public override string ToString()
		{
			var elementType = ElementType;
			return string.Format("{{BindingGroup of {0}}}", elementType == null ? "Unknown Type" : elementType.FullName);
		}

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		void OnBindingGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, e);
			}
		}

		#endregion

		#region IDisposable Members
		private bool _disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				if (Items is INotifyCollectionChanged)
			{
				((INotifyCollectionChanged)Items).CollectionChanged -= OnBindingGroupCollectionChanged;
			}
			}

			_disposed = true;
		}

		~BindingGroup()
		{
			Dispose(false);
		}

		#endregion
	}
}
