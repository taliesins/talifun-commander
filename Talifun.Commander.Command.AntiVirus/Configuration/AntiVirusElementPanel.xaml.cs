using System.Windows;
using System.Windows.Forms;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
    /// <summary>
    /// Interaction logic for AntiVirusElementPanel.xaml
    /// </summary>
    public partial class AntiVirusElementPanel : ElementPanelBase
    {
        public AntiVirusElementPanel()
        {
            Settings = AntiVirusConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private AntiVirusElementPanelDataModel DataModel { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is AntiVirusElement)) return;
			var element = e.Element as AntiVirusElement;

			DataModel = new AntiVirusElementPanelDataModel(element);
			this.DataContext = DataModel;
		}
    }
}
