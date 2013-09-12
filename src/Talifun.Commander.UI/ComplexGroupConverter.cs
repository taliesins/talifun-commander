using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace Talifun.Commander.UI
{
	/// <summary>
	/// ComplexGroupConverter is a IMultiValueConverter used in MultiBindings
	/// in conjuction with the <see>ComplexGroupDataTemplateSelector</see> to
	/// enable complex data template hierachies
	/// 
	/// example usage:
	/// 
	/// ...
	/// &lt;complex:ComplexGroupConverter x:Key="group-converter"/>
	/// &lt;HierarchicalDataTemplate DataType="{x:Type cpn:INet}">
	///     &lt;HierarchicalDataTemplate.ItemsSource>
	///        &lt;MultiBinding Converter="{StaticResource group-converter}">
	///              &lt;Binding Path="Definitions"/>
	///              &lt;Binding Path="Pages"/>
	///        &lt;/MultiBinding>
	///        &lt;/HierarchicalDataTemplate.ItemsSource>
	///        &lt;StackPanel Orientation="Horizontal">
	///            &lt;Image Source="net.png" VerticalAlignment="Center"/>
	///            &lt;TextBlock Text="{Binding Path=Label}"/>
	///        &lt;/StackPanel>
	/// &lt;/HierarchicalDataTemplate>
	/// 
	/// </summary>
	public class ComplexGroupConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var results = new ObservableCollection<object>();
			string[] parameters;
			if (parameter is string)
			{
				parameters = ((string)parameter).Split(',');
				for (var i = 0; i < parameters.Length; i++)
				{
					parameters[i] = parameters[i].Trim();
				}
			}
			else
			{
				parameters = new string[0];
			}
			var index = 0;
			foreach (var value in values)
			{
				if (value is IEnumerable)
				{
					results.Add(index < parameters.Length
									? new BindingGroup(value as IEnumerable, parameters[index])
									: value);
				}
				else
				{
					results.Add(value);
				}
				index++;
			}

			return results;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is ObservableCollection<object>)
			{
				var objects = value as ObservableCollection<object>;
				return objects.ToArray();
			}

			throw new NotSupportedException();
		}

		#endregion
	}
}
