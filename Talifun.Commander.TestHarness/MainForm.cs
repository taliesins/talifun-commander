using System;
using System.Windows.Forms;
using Talifun.Commander.Command;

namespace Talifun.Commander.TestHarness
{
    public partial class MainForm : Form
    {
        private ICommanderManager _mCommanderManager;
        public MainForm()
        {
            InitializeComponent();
            _mCommanderManager = CommanderManagerFactory.Instance.CreateCommandManager();
            SetRunningState(_mCommanderManager.IsRunning);
        }

        private void SetRunningState(bool isRunning)
        {
            StartButton.Enabled = !isRunning;
            StopButton.Enabled = isRunning;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            _mCommanderManager.Start();
            SetRunningState(true);
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            _mCommanderManager.Stop();
            SetRunningState(false);
        }
    }
}
