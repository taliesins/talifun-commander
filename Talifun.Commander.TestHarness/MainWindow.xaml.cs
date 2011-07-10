using System.Windows;
using Talifun.Commander.Command;
using Talifun.Commander.Command.Configuration;

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
            var commanderSectionWindow = new CommanderSectionWindow(_commanderManager);
            commanderSectionWindow.ShowDialog();   
        }
    }
}
