using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
	public class AntiVirusElementPanelDataModel : DataModelBase
	{
		public AntiVirusElementPanelDataModel(AntiVirusElement element)
		{
			Element = element;
		}

		private AntiVirusElement _element;
		public AntiVirusElement Element 
		{ 
			get { return _element; }
			set
			{
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
