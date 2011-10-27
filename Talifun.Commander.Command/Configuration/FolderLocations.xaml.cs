using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using Talifun.Commander.UI;
using UserControl = System.Windows.Controls.UserControl;

namespace Talifun.Commander.Command.Configuration
{
	/// <summary>
	/// Interaction logic for FolderLocations.xaml
	/// </summary>
	[Description("Displays path configuration for command element base.")]
	public partial class FolderLocations : UserControl
	{
		private static readonly DependencyProperty WorkingPathProperty = DependencyProperty.Register("WorkingPath", typeof(string), typeof(FolderLocations), new FrameworkPropertyMetadata(string.Empty));
		private static readonly DependencyProperty ErrorProcessingPathProperty = DependencyProperty.Register("ErrorProcessingPath", typeof(string), typeof(FolderLocations), new FrameworkPropertyMetadata(string.Empty));
		private static readonly DependencyProperty OutPutPathProperty = DependencyProperty.Register("OutPutPath", typeof(string), typeof(FolderLocations), new FrameworkPropertyMetadata(string.Empty));
		private static readonly DependencyProperty FileNameFormatProperty = DependencyProperty.Register("FileNameFormat", typeof(string), typeof(FolderLocations), new FrameworkPropertyMetadata(string.Empty));

		public FolderLocations()
		{
			InitializeComponent();
		}

		[Description("The path where temp files are created while processing a command."), Category("Common Properties")] 
		public string WorkingPath
		{
			get { return base.GetValue(WorkingPathProperty) as string; }
			set { base.SetValue(WorkingPathProperty, value); }
		}

		[Description("The path where files that have failed processing are placed."), Category("Common Properties")] 
		public string ErrorProcessingPath
		{
			get { return base.GetValue(ErrorProcessingPathProperty) as string; }
			set { base.SetValue(ErrorProcessingPathProperty, value); }
		}

		[Description("The path where files that have succedded processing are placed."), Category("Common Properties")] 
		public string OutPutPath
		{
			get { return base.GetValue(OutPutPathProperty) as string; }
			set { base.SetValue(OutPutPathProperty, value); }
		}

		[Description("The string formatting to apply to the outputted file. This excludes the file extension. Uses standard string.format syntax."), Category("Common Properties")] 
		public string FileNameFormat
		{
			get { return base.GetValue(FileNameFormatProperty) as string; }
			set { base.SetValue(FileNameFormatProperty, value); }
		}

		private void workingPathButton_Click(object sender, RoutedEventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog
			{
				SelectedPath = workingPathTextBox.Text
			};

			var result = folderBrowserDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = folderBrowserDialog.SelectedPath;
			workingPathTextBox.Text = foldername;
		}

		private void errorProcessingPathButton_Click(object sender, RoutedEventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog
			{
				SelectedPath = errorProcessingPathTextBox.Text
			};

			var result = folderBrowserDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = folderBrowserDialog.SelectedPath;
			errorProcessingPathTextBox.Text = foldername;
		}

		private void outputPathButton_Click(object sender, RoutedEventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog
			{
				SelectedPath = outputPathTextBox.Text
			};

			var result = folderBrowserDialog.ShowDialog(this.GetIWin32Window());
			if (result != DialogResult.OK) return;

			var foldername = folderBrowserDialog.SelectedPath;
			outputPathTextBox.Text = foldername;
		}
	}
}
