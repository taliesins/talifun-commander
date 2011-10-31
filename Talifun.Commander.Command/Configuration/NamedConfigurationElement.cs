using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Defines an abstract base class for configuration elements that can be contained within sections that derive from
    /// <see cref="ConfigurationElementCollection" />.
    /// </summary>
	public abstract class NamedConfigurationElement : ConfigurationElement, INotifyPropertyChanged, INotifyPropertyChanging 
    {
		public ISettingConfiguration Setting { get; protected set; }

        /// <summary>
        /// Gets or sets a string containing the name of this configuration element.
        /// </summary>
        public abstract string Name
        {
            get;
            set;
        }

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
			//validate the property name in debug builds
			VerifyProperty(propertyName);

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
			//validate the property name in debug builds
			VerifyProperty(propertyName);

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

		public bool SetPropertyValue<T>(T value, Func<T, bool> isValueNewFunction, Action<T> setValueAction, string propertyName)
		{
			if (isValueNewFunction(value))
				return false;

			OnPropertyChanging(propertyName);

			setValueAction(value);

			OnPropertyChanged(propertyName);

			return true;
		}

		public bool SetPropertyValue(object value, ConfigurationProperty configurationProperty, string propertyName)
		{
			if (value.Equals(base[configurationProperty]))
				return false;

			OnPropertyChanging(propertyName);

			base[configurationProperty] = value;

			OnPropertyChanged(propertyName);

			return true;
		}

		public new bool IsModified
		{
			get { return base.IsModified(); }
		}
	}
}
