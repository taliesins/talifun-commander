using System;
using System.Windows.Forms;
using Talifun.Commander.Command;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.TestHarness
{
    public partial class MainForm : Form
    {
        private readonly ICommanderManager _commanderManager;
        private CommanderSectionForm _commanderSectionForm;
        public MainForm()
        {
            InitializeComponent();
            _commanderManager = CommanderManagerFactory.Instance.CreateCommandManager();
            SetRunningState(_commanderManager.IsRunning);
        }

        private void SetRunningState(bool isRunning)
        {
            StartButton.Enabled = !isRunning;
            StopButton.Enabled = isRunning;
        }

        private void OnStartButtonClick(object sender, EventArgs e)
        {
            _commanderManager.Start();
            SetRunningState(true);
        }

        private void OnStopButtonClick(object sender, EventArgs e)
        {
            _commanderManager.Stop();
            SetRunningState(false);
        }

        private void OnSettingsButtonClick(object sender, EventArgs e)
        {
            if (_commanderSectionForm == null)
            {
                _commanderSectionForm = new CommanderSectionForm(_commanderManager);
            }

            _commanderSectionForm.ShowDialog(this);
        }
    }
}
