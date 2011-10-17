using System.Diagnostics;
using System.Windows;
using NLog;
using Talifun.Commander.Command;
using Talifun.Commander.UI;

namespace Talifun.Commander.TestHarness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
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
			_logger.Info("Talifun Commander service starting");
            _commanderManager.Start();
            SetRunningState(true);
			_logger.Info("Talifun Commander service started");
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
			_logger.Info("Talifun Commander service stopping");
            _commanderManager.Stop();
            SetRunningState(false);
			_logger.Info("Talifun Commander service stopped");
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
        	var commanderSectionWindow = _commanderManager.GetCommanderSectionWindow();
        	commanderSectionWindow.ShowDialog();
        }

		private void DeleteEventLogButton_Click(object sender, RoutedEventArgs e)
		{
			if (EventLog.SourceExists(Properties.Resources.LogSource))
			{
				EventLog.DeleteEventSource(Properties.Resources.LogSource);
			}
		}

		private void AddEventLogButton_Click(object sender, RoutedEventArgs e)
		{
			if (!EventLog.SourceExists(Properties.Resources.LogSource))
			{
				EventLog.CreateEventSource(Properties.Resources.LogSource, Properties.Resources.LogName);
			}
		}
    }
}
