using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
	/// <summary>
	/// Interaction logic for CommandLineElementCollectionPanel.xaml
	/// </summary>
	public partial class CommandLineElementCollectionPanel : ElementCollectionPanelBase
	{
		public CommandLineElementCollectionPanel()
		{
			Settings = CommandLineConfiguration.Instance;
			InitializeComponent();
			BindToElementCollection += OnBindToElementCollection;
		}

		private CommandLineElementCollectionPanelDataModel DataModel { get; set; }

		private void OnBindToElementCollection(object sender, BindToElementCollectionEventArgs e)
		{
			DataModel = new CommandLineElementCollectionPanelDataModel(e.AppSettings);
			this.DataContext = DataModel;
		}
	}
}
