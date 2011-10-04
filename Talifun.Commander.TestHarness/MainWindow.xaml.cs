using System.Windows;
using Talifun.Commander.Command;
using Talifun.Commander.UI;

namespace Talifun.Commander.TestHarness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ICommanderManager _commanderManager;
        
        public MainWindow()
        {
            InitializeComponent();
            this.Icon = Command.Properties.Resource.Commander.ToBitmap().ToBitmapSource();
            _commanderManager = CommanderManagerFactory.Instance.CreateCommandManager();
            SetRunningState(_commanderManager.IsRunning);
        }

        private void SetRunningState(bool isRunning)
        {
            StartButton.IsEnabled = !isRunning;
            StopButton.IsEnabled = isRunning;
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            _commanderManager.Start();
            SetRunningState(true);
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            _commanderManager.Stop();
            SetRunningState(false);
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
        	var commanderSectionWindow = _commanderManager.GetCommanderSectionWindow();
        	commanderSectionWindow.ShowDialog();
        }
    }
}
