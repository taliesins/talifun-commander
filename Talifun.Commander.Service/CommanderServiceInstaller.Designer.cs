using Talifun.Commander.Service.Properties;

namespace Talifun.Commander.Service
{
    partial class CommandServiceInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CommanderProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.CommanderInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // CommanderProcessInstaller
            // 
            this.CommanderProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.CommanderProcessInstaller.Password = null;
            this.CommanderProcessInstaller.Username = null;
            // 
            // CommanderInstaller
            // 
            this.CommanderInstaller.Description = Resource.CommanderInstallerDescription;
            this.CommanderInstaller.DisplayName = Resource.CommanderInstallerDisplayName;
            this.CommanderInstaller.ServiceName = Resource.CommanderInstallerServiceName;
            this.CommanderInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // CommanderServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.CommanderProcessInstaller,
            this.CommanderInstaller});
        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller CommanderProcessInstaller;
        private System.ServiceProcess.ServiceInstaller CommanderInstaller;
    }
}