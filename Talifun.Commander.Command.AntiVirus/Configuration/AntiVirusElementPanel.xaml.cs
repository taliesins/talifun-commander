using Talifun.Commander.Command.Configuration;

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

		private AntiVirusElement Element { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is AntiVirusElement)) return;
			Element = e.Element as AntiVirusElement;

			this.DataContext = Element;
		}
    }
}
