using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Talifun.Commander.UI
{
	/// <summary>
	/// ComplexGroupDataTemplateSelector provides additional functionality for finding DataTemplates in WPF
	/// 
	/// </summary>
	public class ComplexGroupDataTemplateSelector : DataTemplateSelector
	{
		public const string DEFAULT_TEMPLATE_KEY_FORMAT = "data-template[{0}]";
		public const string DEFAULT_GROUP_TEMPLATE_KEY_FORMAT = "IEnumerable[{0}]";
		public const DiscoveryMethods DEFAULT_DISCOVERY_METHOD = DiscoveryMethods.Key | DiscoveryMethods.Type | DiscoveryMethods.Interface | DiscoveryMethods.Hierarchy;
		public string TemplateKeyFormat { get; set; }
		public string GroupTemplateKeyFormat { get; set; }
		private readonly Dictionary<object, DataTemplate> _cachedDataTemplates;
		#region DiscoveryMethod
		[Flags]
		public enum DiscoveryMethods
		{
			/// <summary>
			/// create a resource key to find the DataTemplate <see>TemplateKeyFormat</see>  <see>GroupTemplateKeyFormat</see>
			/// </summary>
			Key = 0x01,
			/// <summary>
			/// use the item type to find the DataTemplate based on it's DataType property
			/// </summary>
			Type = 0x02,
			/// <summary>
			/// scan the item types interfaces to find the DataTemplate based on it's DataType property
			/// </summary>
			Interface = 0x04,
			/// <summary>
			/// scan the item types type hierachy to find the DataTemplate based on it's DataType property
			/// </summary>
			Hierarchy = 0x08,
			/// <summary>
			/// look for the DataTemplate in the application resources first
			/// (Overrides the default resource finding method;
			/// </summary>
			GeneralToSpecific = 0x100,
			/// <summary>
			/// use the full type name when creating a resource key using <see>TemplateKeyFormat</see>  <see>GroupTemplateKeyFormat</see>
			/// If not set the unqualified type name will be used.
			/// The default is to use the unqualified type name
			/// </summary>
			FullTypeName = 0x400,
			/// <summary>
			/// Does not cache the result of the resource search.
			/// Note: using this flag can impact performance.
			/// </summary>
			NoCache = 0x800,
		}
		public DiscoveryMethods DiscoveryMethod { get; set; }
		#endregion

		/// <summary>
		/// Internally cached data template to indicate that 
		/// the cached key was already searched and nothing was found.
		/// (Prevents additional useless searches)
		/// </summary>
		private class NullDataTemplate : DataTemplate
		{
			public static readonly NullDataTemplate Instance = new NullDataTemplate();
			private NullDataTemplate()
			{
			}
		}

		public ComplexGroupDataTemplateSelector()
		{
			_cachedDataTemplates = new Dictionary<object, DataTemplate>();
			this.DiscoveryMethod = DEFAULT_DISCOVERY_METHOD;
			this.TemplateKeyFormat = DEFAULT_TEMPLATE_KEY_FORMAT;
			this.GroupTemplateKeyFormat = DEFAULT_GROUP_TEMPLATE_KEY_FORMAT;
		}

		/// <summary>
		/// Selects a template by resource key looking from the specific to the general containers in the application heirachy
		/// The resource key to attempted to be found in the following order:
		/// 
		/// Application resources
		/// The main window resources
		/// The active window resources
		/// The container resources
		/// 
		/// </summary>
		/// <param name="resourceKey">The resource key to look up</param>
		/// <param name="container">The container to start looking in</param>
		/// <returns></returns>
		private DataTemplate SelectByKeyGeneralToSpecific(object resourceKey, DependencyObject container)
		{
			DataTemplate dataTemplate = null;
			dataTemplate = Application.Current.TryFindResource(resourceKey) as DataTemplate;
			if (dataTemplate == null)
			{
				dataTemplate = Application.Current.MainWindow.TryFindResource(resourceKey) as DataTemplate;
			}
			if (dataTemplate == null)
			{
				foreach (Window window in Application.Current.Windows)
				{
					if (window.IsActive)
					{
						dataTemplate = window.TryFindResource(resourceKey) as DataTemplate;
						break;
					}
				}
			}
			if (dataTemplate == null)
			{
				if (container is FrameworkElement)
				{
					dataTemplate = ((FrameworkElement)container).TryFindResource(resourceKey) as DataTemplate;
				}
			}
			return dataTemplate;
		}

		/// <summary>
		/// Selects a template by resource key.
		/// Where to look is based on the DiscoverMethod property
		/// 
		/// </summary>
		/// <param name="resourceKey">The resource key to look up</param>
		/// <param name="container">The container to start looking in</param>
		/// <returns></returns>
		private DataTemplate SelectByKey(object resourceKey, DependencyObject container)
		{
			if ((this.DiscoveryMethod & DiscoveryMethods.GeneralToSpecific) != 0)
			{
				return SelectByKeyGeneralToSpecific(resourceKey, container);
			}
			else
			{
				if (container is FrameworkElement)
				{
					return ((FrameworkElement)container).TryFindResource(resourceKey) as DataTemplate;
				}
				else
				{
					return null;
				}
			}
		}
		/// <summary>
		/// Selects a data template, through cache, by key
		/// </summary>
		/// <param name="templateKey">The template key to look for</param>
		/// <param name="container">The items container</param>
		/// <returns>The selected DataTemplate or null if not found</returns>
		private DataTemplate SelectThroughCacheByKey(object templateKey, DependencyObject container)
		{
			DataTemplate dataTemplate = null;
			if (!_cachedDataTemplates.TryGetValue(templateKey, out dataTemplate))
			{
				dataTemplate = SelectByKey(templateKey, container);
				if (dataTemplate == null)
				{
					_cachedDataTemplates.Add(templateKey, NullDataTemplate.Instance);
				}
				else
				{
					_cachedDataTemplates.Add(templateKey, dataTemplate);
				}
			}
			else if (dataTemplate is NullDataTemplate)
			{
				return null;
			}
			return dataTemplate;
		}

		/// <summary>
		/// selects a DataTemplate by scanning the Type hierachy using a DataTemplateKey
		/// </summary>
		/// <param name="type">The item type to look for</param>
		/// <param name="container">The items container</param>
		/// <returns>The selected DataTemplate or null if not found</returns>
		private DataTemplate SelectByTypeHierachy(Type type, FrameworkElement container)
		{
			DataTemplate dataTemplate = null;
			while (dataTemplate == null && type != typeof(object))
			{
				DataTemplateKey dataTemplateKey = new DataTemplateKey(type);
				dataTemplate = SelectByKey(dataTemplateKey, container);
				type = type.BaseType;
			}
			return dataTemplate;
		}

		/// <summary>
		/// Selects a data template by type, interface, or hiearchy (Depending on the DiscoveryMethod property
		/// </summary>
		/// <param name="itemType">The item type to look for</param>
		/// <param name="container">The items container</param>
		/// <returns>The selected DataTemplate or null if not found</returns>
		private DataTemplate SelectByType(Type itemType, FrameworkElement container)
		{
			DataTemplate dataTemplate = null;
			if (container == null)
			{
				return null;
			}
			if ((this.DiscoveryMethod & DiscoveryMethods.Type) == DiscoveryMethods.Type)
			{
				DataTemplateKey dataTemplateKey = new DataTemplateKey(itemType);
				dataTemplate = SelectByKey(dataTemplateKey, container);
			}
			if (dataTemplate == null)
			{
				if ((this.DiscoveryMethod & DiscoveryMethods.Interface) == DiscoveryMethods.Interface)
				{
					Type[] interfaces = itemType.GetInterfaces();
					for (int i = interfaces.Length - 1; i >= 0; i--)
					{
						Type interfaceType = interfaces[i];
						DataTemplateKey dataTemplateKey = new DataTemplateKey(interfaceType);
						dataTemplate = SelectByKey(dataTemplateKey, container);
						if (dataTemplate != null)
						{
							break;
						}
					}
				}
			}
			if (dataTemplate == null)
			{
				if ((this.DiscoveryMethod & DiscoveryMethods.Hierarchy) == DiscoveryMethods.Hierarchy)
				{
					dataTemplate = SelectByTypeHierachy(itemType.BaseType, container);
				}
			}
			return dataTemplate;
		}

		/// <summary>
		/// Selects a data template, through cache, by type, interface, or hiearchy (Depending on the DiscoveryMethod property
		/// </summary>
		/// <param name="itemType">The item type to look for</param>
		/// <param name="container">The items container</param>
		/// <returns>The selected DataTemplate or null if not found</returns>
		private DataTemplate SelectThroughCacheByType(Type itemType, FrameworkElement container)
		{
			DataTemplate dataTemplate = null;
			DataTemplateKey dataTemplateKey = new DataTemplateKey(itemType);
			if (!_cachedDataTemplates.TryGetValue(dataTemplateKey, out dataTemplate))
			{
				dataTemplate = SelectByType(itemType, container);
				if (dataTemplate == null)
				{
					_cachedDataTemplates.Add(dataTemplateKey, NullDataTemplate.Instance);
				}
				else
				{
					_cachedDataTemplates.Add(dataTemplateKey, dataTemplate);
				}
			}
			else if (dataTemplate is NullDataTemplate)
			{
				return null;
			}
			return dataTemplate;
		}

		private string GetTypeNameForKey(Type type)
		{
			if ((this.DiscoveryMethod & DiscoveryMethods.FullTypeName) != 0)
			{
				return type.FullName;
			}
			else
			{
				return type.Name;
			}
		}

		/// <summary>
		/// override for DataTemplate SelectTemplate method
		/// </summary>
		/// <param name="item">The item to be templated</param>
		/// <param name="container">The items container</param>
		/// <returns>The selected DataTemplate</returns>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			string templateKey = null;
			DataTemplate dataTemplate = null;
			if (item == null || container == null)
			{
				return null;
			}
			try
			{
				if ((this.DiscoveryMethod & DiscoveryMethods.Key) == DiscoveryMethods.Key)
				{
					if (item is IBindingGroup)
					{
						IBindingGroup bindingGroup = item as IBindingGroup;
						Type elementType = bindingGroup.ElementType;
						if (elementType != null)
						{
							if (bindingGroup.Parameter == null)
							{
								templateKey = string.Format(this.GroupTemplateKeyFormat, GetTypeNameForKey(elementType));
							}
							else
							{
								templateKey = bindingGroup.Parameter;
							}
							if ((this.DiscoveryMethod & DiscoveryMethods.NoCache) != 0)
							{
								dataTemplate = SelectByKey(templateKey, container);
							}
							else
							{
								dataTemplate = SelectThroughCacheByKey(templateKey, container);
							}
						}
					}
					else if (item is IEnumerable)
					{
						Type elementType = BindingGroup.GetElementType(item as IEnumerable);
						if (elementType != null)
						{
							templateKey = string.Format(this.GroupTemplateKeyFormat, GetTypeNameForKey(elementType));
							if ((this.DiscoveryMethod & DiscoveryMethods.NoCache) != 0)
							{
								dataTemplate = SelectByKey(templateKey, container);
							}
							else
							{
								dataTemplate = SelectThroughCacheByKey(templateKey, container);
							}
						}
					}
					else
					{
						templateKey = string.Format(this.TemplateKeyFormat, GetTypeNameForKey(item.GetType()));
						if ((this.DiscoveryMethod & DiscoveryMethods.NoCache) != 0)
						{
							dataTemplate = SelectByKey(templateKey, container);
						}
						else
						{
							dataTemplate = SelectThroughCacheByKey(templateKey, container);
						}
					}
				}

				if (dataTemplate == null)
				{
					if ((this.DiscoveryMethod & (DiscoveryMethods.Type | DiscoveryMethods.Interface | DiscoveryMethods.Hierarchy)) != 0)
					{
						if ((this.DiscoveryMethod & DiscoveryMethods.NoCache) != 0)
						{
							dataTemplate = SelectByType(item.GetType(), container as FrameworkElement);
						}
						else
						{
							dataTemplate = SelectThroughCacheByType(item.GetType(), container as FrameworkElement);
						}
					}
				}
#if DEBUG
				if (dataTemplate == null)
				{
					string debugMessage = "DataTemplate not found for";
					if ((this.DiscoveryMethod & DiscoveryMethods.Key) != 0)
					{
						debugMessage += " Resource Key \"" + templateKey + "\"";
					}
					if ((this.DiscoveryMethod & (DiscoveryMethods.Type | DiscoveryMethods.Interface | DiscoveryMethods.Hierarchy)) != 0)
					{
						if ((this.DiscoveryMethod & DiscoveryMethods.Key) != 0)
						{
							debugMessage += " or";
						}
						debugMessage += " DataType \"" + item.GetType().FullName + "\"";
					}
					Debug.WriteLine(debugMessage, GetType().Name);
				}
#endif
			}
			catch
			{
				dataTemplate = base.SelectTemplate(item, container);
			}
			return dataTemplate;
		}
	}
}
