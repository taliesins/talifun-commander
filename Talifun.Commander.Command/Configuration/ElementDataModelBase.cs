namespace Talifun.Commander.Command.Configuration
{
	public class ElementDataModelBase<T> : DataModelBase where T : CommandConfigurationBase
	{
		public ElementDataModelBase(T element)
		{
			Element = element;
		}

		private T _element;
		public T Element 
		{ 
			get { return _element; }
			set
			{
				if (_element == value) return;

				OnPropertyChanging("Element");

				if (_element != null)
				{
					_element.PropertyChanging -= OnElementPropertyChanging;
					_element.PropertyChanged -= OnElementPropertyChanged;
				}

				_element = value;

				if (_element != null)
				{
					_element.PropertyChanging += OnElementPropertyChanging;
					_element.PropertyChanged += OnElementPropertyChanged;
				}

				OnPropertyChanged("Element");
			}
		}

		private void OnElementPropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
		{
			OnPropertyChanging("Element");
		}

		private void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnPropertyChanged("Element");
		}
	}
}
