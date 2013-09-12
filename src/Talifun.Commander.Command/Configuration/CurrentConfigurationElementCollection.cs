using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Configuration
{
	public abstract class CurrentConfigurationElementList : ConfigurationElementCollection, IList, INotifyCollectionChanged, INotifyPropertyChanged
	{
		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, e);
			}
		}

		#endregion

		#region INotifyPropertyChanged Members

		protected const string CountString = "Count";
		protected const string IndexerName = "Item[]";

		///<summary>
		///Occurs when a property value has been changed.
		///</summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Occurs when a property value is about to be changed.
		/// </summary>
		public event PropertyChangingEventHandler PropertyChanging;

		/// <summary>
		/// Raises the <see cref="PropertyChanged"/> event for
		/// a given property.
		/// </summary>
		/// <param name="propertyName">The name of the changed property.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			if (propertyName != IndexerName)
			{
				//validate the property name in debug builds
				VerifyProperty(propertyName);
			}

			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Raises the <see cref="PropertyChanged"/> event for
		/// a given property.
		/// </summary>
		/// <param name="propertyName">The name of the changed property.</param>
		protected void OnPropertyChanging(string propertyName)
		{
			if (propertyName != IndexerName)
			{
				//validate the property name in debug builds
				VerifyProperty(propertyName);
			}

			if (PropertyChanging != null)
			{
				PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Verifies whether the current class provides a property with a given
		/// name. This method is only invoked in debug builds, and results in
		/// a runtime exception if the <see cref="OnPropertyChanged"/> method
		/// is being invoked with an invalid property name. This may happen if
		/// a property's name was changed but not the parameter of the property's
		/// invocation of <see cref="OnPropertyChanged"/>.
		/// </summary>
		/// <param name="propertyName">The name of the changed property.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
			MessageId = "System.String.Format(System.String,System.Object,System.Object)"), Conditional("DEBUG")]
		[DebuggerNonUserCode]
		private void VerifyProperty(string propertyName)
		{
			var type = GetType();

			//look for a *public* property with the specified name
			var pi = type.GetProperty(propertyName);
			if (pi != null) return;
			//there is no matching property - notify the developer
			var msg = String.Format(Resource.ErrorMessageOnPropertyChangedInvokedWithInvalidPropertyName, propertyName, type.FullName);
			Debug.Fail(msg);
		}

		#endregion

		#region IList Members

		int IList.Add(object value)
		{
			var item = (NamedConfigurationElement)value;
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			this.BaseAdd(item);
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			var index = this.BaseIndexOf(item);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));

			return index;
		}

		void IList.Clear()
		{
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			base.BaseClear();
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		bool IList.Contains(object value)
		{
			return (base.BaseIndexOf((NamedConfigurationElement)value) > -1);
		}

		int IList.IndexOf(object value)
		{
			return base.BaseIndexOf((NamedConfigurationElement)value);
		}

		void IList.Insert(int index, object value)
		{
			var item = (NamedConfigurationElement)value;
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			this.BaseAdd(index, item);
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		void IList.RemoveAt(int index)
		{
			var item = base.BaseGet(index);
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			base.BaseRemoveAt(index);
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
		}

		void IList.Remove(object value)
		{
			var index = base.BaseIndexOf((NamedConfigurationElement)value);
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			base.BaseRemoveAt(index);
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool IList.IsReadOnly
		{
			get { return base.IsReadOnly(); }
		}

		public object this[int index]
		{
			get { return base.BaseGet(index) as NamedConfigurationElement; }
			set
			{
				var item = (NamedConfigurationElement) value;
				var configurationElement = base.BaseGet(index);
				if (base.BaseGet(index) == null)
				{
					this.OnPropertyChanging(IndexerName);
					this.OnPropertyChanging(CountString);
					this.BaseAdd(index, item);
					this.OnPropertyChanged(CountString);
					this.OnPropertyChanged(IndexerName);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
				}
				else
				{
					this.OnPropertyChanging(IndexerName);
					base.BaseRemoveAt(index);
					this.BaseAdd(index, item);
					this.OnPropertyChanged(IndexerName);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, configurationElement, index));
				}
			}
		}

		#endregion

		#region ConfigurationElementCollection
		public ISettingConfiguration Setting { get; protected set; }

		/// <summary>
		/// Creates a new instance of Setting.ElementType.
		/// </summary>
		/// <returns>
		/// A new configuration element.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return (ConfigurationElement)Activator.CreateInstance(Setting.ElementType);
		}

		/// <summary>
		/// Gets the element key for a specified configuration element.
		/// </summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for.</param>
		/// <returns>
		/// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"></see>.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">Thrown when the specified <see cref="ConfigurationElement" /> does not expose <c>Name</c> as a property.</exception>
		protected override object GetElementKey(ConfigurationElement element)
		{
			var namedElement = element as NamedConfigurationElement;
			if (null != namedElement) return (namedElement.Name);
			throw (new InvalidOperationException("ConfigurationElement does not expose Name as a property"));
		}
		#endregion
	}

    [InheritedExport]
	[Serializable]
	public abstract class CurrentConfigurationElementCollection : CurrentConfigurationElementList
    {
        /// <summary>
        /// The collection of instances of classes derived from <see cref="ConfigurationElement" />  that represent the configuration properties containing in this collection.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Visibility is limted to derived classes which should close the generic type and treat the field as private")]
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Visibility is limted to derived classes which should close the generic type and treat the field as private")]
        protected static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

        /// <summary>
        /// Gets the collection of configuration properties contained by this configuration element collection.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Gets the <see cref="ConfigurationElementCollectionType" /> of the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public abstract ConfigurationProperty CreateNewConfigurationProperty();

		/// <summary>
		/// Remove an element from the collection.
		/// </summary>
		/// <param name="index">The index of the element to remove from the collection.</param>
		public bool Remove(int index)
		{
			var configurationElement = base.BaseGet(index);
			if (base.BaseGet(index) == null) return false;
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			base.BaseRemoveAt(index);
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, configurationElement, index));
			return true;
		}

		/// <summary>
		/// Remove an element from the collection.
		/// </summary>
		/// <param name="name">The name of the element to remove from the collection.</param>
		public bool Remove(string name)
		{
			var configurationElement = base.BaseGet(name);
			if (configurationElement == null) return false;
			var index = base.BaseIndexOf(configurationElement);
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			base.BaseRemoveAt(index);
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, configurationElement, index));
			return true;
		}

		/// <summary>
		/// Gets or sets the configuration element at the specified index.
		/// </summary>
		new public NamedConfigurationElement this[int index]
		{
			get { return base.BaseGet(index) as NamedConfigurationElement; }
			set
			{
				var configurationElement = base.BaseGet(index);
				if (base.BaseGet(index) == null)
				{
					this.OnPropertyChanging(IndexerName);
					this.OnPropertyChanging(CountString);
					this.BaseAdd(index, value);
					this.OnPropertyChanged(CountString);
					this.OnPropertyChanged(IndexerName);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
				}
				else
				{
					this.OnPropertyChanging(IndexerName);
					base.BaseRemoveAt(index);
					this.BaseAdd(index, value);
					this.OnPropertyChanged(IndexerName);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, configurationElement, index));
				}
			}
		}

		/// <summary>
		/// Gets or sets the configuration element with the specified name.
		/// </summary>
		new public NamedConfigurationElement this[string name]
		{
			get { return base.BaseGet(name) as NamedConfigurationElement; }
			set
			{
				var configurationElement = base.BaseGet(name);
				if (configurationElement == null)
				{
					this.OnPropertyChanging(IndexerName);
					this.OnPropertyChanging(CountString);
					this.BaseAdd(value);
					this.OnPropertyChanged(CountString);
					this.OnPropertyChanged(IndexerName);
					var index = this.BaseIndexOf(value);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
				}
				else
				{
					this.OnPropertyChanging(IndexerName);
					base.BaseRemove(name);
					this.BaseAdd(value);
					this.OnPropertyChanged(IndexerName);
					var index = this.BaseIndexOf(value);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, configurationElement, index));
				}
			}
		}

		/// <summary>
		/// Creates a new instance of Setting.ElementType.
		/// </summary>
		/// <returns>
		/// A new named configuration element.
		/// </returns>
		public NamedConfigurationElement CreateNew()
		{
			return (NamedConfigurationElement)Activator.CreateInstance(Setting.ElementType);
		}
	}

	public interface ICurrentConfigurationElementCollection<T> where T : NamedConfigurationElement, new()
	{
		/// <summary>
		/// Gets or sets the configuration element with the specified name.
		/// </summary>
		T this[string name] { get; set; }

		/// <summary>
		/// Gets or sets the configuration element at the specified index.
		/// </summary>
		 T this[int index] { get; set; }

		bool IsModified { get; }
		int Count { get; }
		bool IsReadOnly { get; }

		/// <summary>
		/// Gets the <see cref="ConfigurationElementCollectionType" /> of the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
		/// </summary>
		ConfigurationElementCollectionType CollectionType { get; }

		ISettingConfiguration Setting { get; }
		bool EmitClear { get; set; }
		bool IsSynchronized { get; }
		object SyncRoot { get; }
		ConfigurationLockCollection LockAttributes { get; }
		ConfigurationLockCollection LockAllAttributesExcept { get; }
		ConfigurationLockCollection LockElements { get; }
		ConfigurationLockCollection LockAllElementsExcept { get; }
		bool LockItem { get; set; }
		ElementInformation ElementInformation { get; }
		System.Configuration.Configuration CurrentConfiguration { get; }

		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/>.
		/// </summary>
		/// <returns>
		/// A new <typeparamref name="T" />.
		/// </returns>
		new T CreateNew();

		int IndexOf(T item);
		void Insert(int index, T item);
		void RemoveAt(int index);
		void Add(T item);
		void Clear();
		bool Contains(T item);
		void CopyTo(T[] array, int arrayIndex);

		IEnumerator<T> GetEnumerator();
		ConfigurationProperty CreateNewConfigurationProperty();

		/// <summary>
		/// Remove an element from the collection.
		/// </summary>
		/// <param name="item">The element to remove from the collection.</param>
		bool Remove(T item);

		/// <summary>
		/// Remove an element from the collection.
		/// </summary>
		/// <param name="index">The index of the element to remove from the collection.</param>
		bool Remove(int index);

		/// <summary>
		/// Remove an element from the collection.
		/// </summary>
		/// <param name="name">The name of the element to remove from the collection.</param>
		bool Remove(string name);

		event NotifyCollectionChangedEventHandler CollectionChanged;

		///<summary>
		///Occurs when a property value has been changed.
		///</summary>
		event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Occurs when a property value is about to be changed.
		/// </summary>
		event PropertyChangingEventHandler PropertyChanging;

		bool Equals(object compareTo);
		int GetHashCode();
		void CopyTo(ConfigurationElement[] array, int index);
	}

	/// <summary>
    /// Defines a generic collection class for strongly-typed configuration elements, where <typeparamref name="T"/> is a type derived from <see cref="System.Configuration.ConfigurationElement" />.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="System.Configuration.ConfigurationElement" />.</typeparam>
	public abstract class CurrentConfigurationElementCollection<T> : CurrentConfigurationElementCollection, IList<T>, ICurrentConfigurationElementCollection<T> where T : NamedConfigurationElement, new()
    {
        /// <summary>
        /// Gets or sets the configuration element with the specified name.
        /// </summary>
        new public T this[string name]
        {
            get { return base.BaseGet(name) as T; }
            set
            {
                var configurationElement = base.BaseGet(name);
                if (configurationElement == null)
                {
					this.OnPropertyChanging(IndexerName);
					this.OnPropertyChanging(CountString);
                    this.BaseAdd(value);
                    this.OnPropertyChanged(CountString);
                    this.OnPropertyChanged(IndexerName);
					var index = this.BaseIndexOf(value);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
                }
                else
                {
					this.OnPropertyChanging(IndexerName);
                    base.BaseRemove(name);
                    this.BaseAdd(value);
                    this.OnPropertyChanged(IndexerName);
					var index = this.BaseIndexOf(value);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, configurationElement, index));
                }
            }
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/>.
        /// </summary>
        /// <returns>
        /// A new <typeparamref name="T" />.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/>.
		/// </summary>
		/// <returns>
		/// A new <typeparamref name="T" />.
		/// </returns>
		public new T CreateNew()
		{
			return (T)CreateNewElement();
		}

		public new bool IsModified
		{
			get { return base.IsModified(); }
		}

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return base.BaseIndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			this.BaseAdd(index, item);
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		public void RemoveAt(int index)
		{
			var item = base.BaseGet(index);
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			base.BaseRemoveAt(index);
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
		}

		/// <summary>
		/// Gets or sets the configuration element at the specified index.
		/// </summary>
		new public T this[int index]
		{
			get { return base.BaseGet(index) as T; }
			set
			{
				var configurationElement = base.BaseGet(index);
				if (base.BaseGet(index) == null)
				{
					this.OnPropertyChanging(IndexerName);
					this.OnPropertyChanging(CountString);
					this.BaseAdd(index, value);
					this.OnPropertyChanged(CountString);
					this.OnPropertyChanged(IndexerName);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
				}
				else
				{
					this.OnPropertyChanging(IndexerName);
					base.BaseRemoveAt(index);
					this.BaseAdd(index, value);
					this.OnPropertyChanged(IndexerName);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, configurationElement, index));
				}
			}
		}

		#endregion

		#region ICollection<T> Members
		/// <summary>
		/// Allows you to add items to the collection with ExandoObject and dynamic objects.
		/// </summary>
		/// <typeparam name="Y"></typeparam>
		/// <param name="item"></param>
		/// <remarks>Use reflection to use this</remarks>
		private void Add<Y>(Y item) where Y : T
		{
			Add(item);
		}

		public void Add(T item)
		{
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			this.BaseAdd(item);
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			var index = this.BaseIndexOf(item);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		public void Clear()
		{
			this.OnPropertyChanging(IndexerName);
			this.OnPropertyChanging(CountString);
			base.BaseClear();
			this.OnPropertyChanged(CountString);
			this.OnPropertyChanged(IndexerName);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public bool Contains(T item)
		{
			return (base.BaseIndexOf(item) > -1);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			base.CopyTo(array, arrayIndex);
		}

		new public int Count
		{
			get { return base.Count; }
		}

		new  public bool IsReadOnly
		{
			get { return base.IsReadOnly(); }
		}

		public bool Remove(T item)
		{
			var index = base.BaseIndexOf(item);
			var result = false;
			if (index > -1)
			{
				this.OnPropertyChanging(IndexerName);
				this.OnPropertyChanging(CountString);
				result = base.Remove(index);
				this.OnPropertyChanged(CountString);
				this.OnPropertyChanged(IndexerName);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
			}

			return result;
		}

		#endregion

		#region IEnumerable<T> Members

		new public IEnumerator<T> GetEnumerator()
		{
			return new WrapperEnumerator(base.GetEnumerator());
		}

		private class WrapperEnumerator : IEnumerator<T>
		{
			private readonly IEnumerator _enumerator;
			public WrapperEnumerator(IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			public T Current
			{
				get { return (T)_enumerator.Current; }
			}

			object IEnumerator.Current
			{
				get { return _enumerator.Current; }
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			public void Dispose()
			{
			}
		}

		#endregion
	}
}
