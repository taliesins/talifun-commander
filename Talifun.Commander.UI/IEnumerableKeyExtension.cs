using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace Talifun.Commander.UI
{
	[MarkupExtensionReturnType(typeof(string))]
	public class IEnumerableKeyExtension : MarkupExtension
	{
		private static readonly Type GenericEnumerableType = typeof(IEnumerable<object>).GetGenericTypeDefinition();
		public Type Type { get; set; }
		public string TypeName { get; set; }
		public IEnumerableKeyExtension()
		{ }
		public IEnumerableKeyExtension(string typeName)
		{
			TypeName = typeName;
		}
		public IEnumerableKeyExtension(Type type)
		{
			Type = type;
		}
		private Type ParseType(IServiceProvider serviceProvider)
		{
			if (Type == null)
			{
				var xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
				if (xamlTypeResolver != null)
				{
					return xamlTypeResolver.Resolve(TypeName);
				}
			}
			else
			{
				return Type;
			}
			return typeof(IEnumerable<object>);
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new DataTemplateKey(GenericEnumerableType.MakeGenericType(ParseType(serviceProvider)));
		}
	}
}
