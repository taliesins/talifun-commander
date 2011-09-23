using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
    /// <summary>
    /// Interaction logic for CommandLineElementPanel.xaml
    /// </summary>
    public partial class CommandLineElementPanel : ElementPanelBase
    {
        public CommandLineElementPanel()
        {
            Settings = CommandLineConfiguration.Instance;
            InitializeComponent();

			BindToElement += OnBindToElement;
        }

		private CommandLineElement Element { get; set; }

		private void OnBindToElement(object sender, BindToElementEventArgs e)
		{
			if (e.Element == null || !(e.Element is CommandLineElement)) return;
			Element = e.Element as CommandLineElement;

			this.DataContext = Element;
		}
    }
}
