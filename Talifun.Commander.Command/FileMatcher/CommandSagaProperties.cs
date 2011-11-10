using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.FileMatcher
{
    public class CommandSagaProperties : ICommandSagaProperties
    {
    	public string InputFilePath { get; set; }
        public ProjectElement Project { get; set; }
        public FileMatchElement FileMatch { get; set; }
		public AppSettingsSection AppSettings { get; set; }
    }
}
